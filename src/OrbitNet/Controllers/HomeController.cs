using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OrbitNet.Models;
using OrbitNet.Models.TDAs;
using OrbitNet.Models.Nodes;
using Microsoft.AspNetCore.Http;
using System.IO; //leer archivos
using System.Xml;
using System.Text.RegularExpressions;
using System.ComponentModel.Design;
using System.ComponentModel;

namespace OrbitNet.Controllers
{
    public class HomeController : Controller
    {
        private static readonly LogAuditoria bitacoraAuditoria = new LogAuditoria();

        // Listas permanentes temporales hasta que el Integrante 1 entregue RedSatelitalPlano
        private static readonly ListaSatelitesEcu baseDatosEcu = new ListaSatelitesEcu();
        private static readonly ListaSatelitesPol baseDatosPol = new ListaSatelitesPol();
        private static readonly ListaAntenas baseDatosAnt = new ListaAntenas();

// TODO FASE 2 - Integrante 1: Reemplazar las 3 listas de arriba con RedSatelitalPlano
// private static readonly RedSatelitalPlano redSatelital = new RedSatelitalPlano();

// TODO FASE 2 - Integrante 2: Agregar RegistroSatelites (AVL) cuando esté listo
// private static readonly RegistroSatelites registroAvl = new RegistroSatelites();

        //Expresiones Regulares Oficiales según el Enunciado del Proyecto
        private const string PatronIdSatelite = @"^SAT-(ECU|POL)-\d{4}$";
        private const string PatronIpv4 = @"^(?:(?:25[0-5]|2[0-4]\d|[01]?\d?\d)\.){3}(?:25[0-5]|2[0-4]\d|[01]?\d?\d)$";
        private const string PatronCoordenadas = @"^-?\d{1,2}\.\d{4,6},-?\d{1,3}\.\d{4,6}$";

        [HttpGet]
        public IActionResult Index()
        {
            //Inicializació de logs básicos de arranque si la bitácora está vacía
            if(bitacoraAuditoria.EstaVacia)
            {
                bitacoraAuditoria.Registrar("INFO", "Sistema de simulación espacial inicializado correctamente");
                bitacoraAuditoria.Registrar("INFO", "Esperando archivo XML de configuración...");
            }

            var viewModel = new DashboardViewModel
            {
                SatelitesEcu = baseDatosEcu,
                SatelitesPol = baseDatosPol,
                Antenas = baseDatosAnt,
                Logs = bitacoraAuditoria
                // TODO FASE 2 - Integrante 1: Agregar RedSatelital al ViewModel
                // RedSatelital = redSatelital
            };
            return View(viewModel);
        }

        //-----> CARGAR XML <----------
        [HttpPost]
        public IActionResult CargarXml(IFormFile archivoXml)
        {
            var viewModel = new DashboardViewModel
            {
                SatelitesEcu = baseDatosEcu,
                SatelitesPol = baseDatosPol,
                Antenas = baseDatosAnt,
                Logs = bitacoraAuditoria
            };

            if(archivoXml == null || archivoXml.Length == 0)
            {
                ViewBag.ErrorMessage = "Por favor, seleccione un archivo XML válido.";
                return View("Index", viewModel);
            }

            bitacoraAuditoria.Registrar("INFO", $"Iniciando carga de archivo: '{archivoXml.FileName}'");

            //
            // 1. Instanciación del TDA Temporal para Carga Transaccional (Atómica)
        // Se acumularán los satélites validados aquí antes de pasarlos a la base de datos principal
        //EN ESTA PARTE ESTA INCOMPLETA, SE COMPLETARA CUANDO LOS DEMAS INTEGRANTES AGREGUEN LO DEMAS
        //------------------>PRUEBAS<------------------------------
        // ── LISTAS TEMPORALES (Patrón Commit/Rollback) ───────────────────
        // Se acumulan los datos validados aquí antes de pasarlos a los TDAs principales
        // Cuando los TDAs de los compañeros estén listos, estas listas
        // temporales se reemplazarán por los TDAs correspondientes
            ListaSatelitesEcu tempEcu = new ListaSatelitesEcu();
            ListaSatelitesPol tempPol = new ListaSatelitesPol();
            ListaAntenas tempAnt = new ListaAntenas();

            bool  transaccionExitosa = true;
            string causaFallo = "";

            try
            {
                //Mitigación de Vulnerabilidades XXE
                XmlReaderSettings settings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Prohibit,
                    XmlResolver = null
                };

                using (Stream stream = archivoXml.OpenReadStream())
                using (XmlReader reader = XmlReader.Create(stream, settings))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(reader);

                    // Selección de elementos mediante XPath
                    //------------->VALIDACIONES PARA SATELITES ECUATORIALES<-------------
                    XmlNodeList satelitesEcua = doc.SelectNodes("/orbitnet/constelaciones_ecuatoriales/satelite")!;
                    if(satelitesEcua.Count == 0)
                    {
                        transaccionExitosa = false;
                        causaFallo = "No se encontraron nodos de satélites bajo el XPath '/orbitnet/constelaciones_ecuatoriales/satelite'.";
                    }
                    else
                    {
                        // 2. Procesamiento de los nodos con validaciones RegEx
                        foreach (XmlNode nodo in satelitesEcua)
                        {
                            string? id= nodo.Attributes?["id"]?.Value?.Trim();
                            string? nombre = nodo.SelectSingleNode("nombre")?.InnerText?.Trim();
                            string? enlaceIp = nodo.SelectSingleNode("enlace_ip")?.InnerText?.Trim();

                            // Comprobamos la existencia física de campos
                            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(enlaceIp))
                            {
                                transaccionExitosa = false;
                                causaFallo = "Satélite ecuatorial con campos vacíos o faltantes.";
                                break;
                            }

                            // --- VALIDACIÓN 1 REGEX: ID del Satélite ---
                            if (!Regex.IsMatch(id, PatronIdSatelite))
                            {
                                transaccionExitosa = false;
                                causaFallo = $"ID inválido '{id}' en <constelaciones_ecuatoriales>. " +
                                             $"Formato requerido: SAT-(ECU|POL)-0000.";
                                break;
                            }

                            // --- VALIDACIÓN 2 REGEX: Dirección IPv4 ---
                            if (!Regex.IsMatch(enlaceIp, PatronIpv4))
                            {
                                transaccionExitosa = false;
                                causaFallo = $"El satélite [{id}] contiene una dirección IP inválida: '{enlaceIp}'.";
                                break;
                            }
                            //se agrega a la lista temporal si pasa las validaciones de encapsulamiento
                            //por el momento lo realizare asi ya que estoy utilizando listas no nativas, las cambiare
                            //cuando se agregue las clases correctas para las listas
                            tempEcu.InsertarAlFinal(new SateliteEcu(id, nombre, enlaceIp));
                        }
                    }

                    //------------->VALIDACIONES PARA SATELITES POLARES<-------------
                    if(transaccionExitosa)
                    {
                        XmlNodeList polares = doc.SelectNodes("/orbitnet/orbitas_polares/polar/satelite")!;

                        if(polares.Count == 0)
                        {
                            transaccionExitosa = false;
                            causaFallo = "No se encontraron satélites polares " +
                                         "bajo '/orbitnet/orbitas_polares/polar/satelite'.";   
                        }
                        else
                        {
                            foreach (XmlNode polar in polares)
                            {
                                string? polarId = polar.Attributes?["id"]?.Value?.Trim();

                                if (string.IsNullOrWhiteSpace(polarId))
                                {
                                    transaccionExitosa = false;
                                    causaFallo = "Órbita porlar con ID vacío o faltante.";
                                    break;
                                }

                                XmlNodeList satelitesPol = polar.SelectNodes("satelite")!;

                                foreach (XmlNode nodo in satelitesPol)
                                {
                                    string? sateliteId = nodo.Attributes?["id"]?.Value?.Trim();
                                    string? nombre = nodo.SelectSingleNode("nombre")?.InnerText?.Trim();
                                    string? frecuencia = nodo.SelectSingleNode("frecuencia")?.InnerText?.Trim();

                                    if (string.IsNullOrWhiteSpace(sateliteId) || string.IsNullOrWhiteSpace(nombre))
                                    {
                                        transaccionExitosa = false;
                                        causaFallo = "Satélite polar con campos vacíos o faltantes.";
                                        break;
                                    }
                                    // --- VALIDACIÓN 1 REGEX: ID del Satélite ---
                                    if (!Regex.IsMatch(sateliteId, PatronIdSatelite))
                                    {
                                        transaccionExitosa = false;
                                        causaFallo = $"ID inválido '{sateliteId}' en <orbitas_polares>. " +
                                             $"Formato requerido: SAT-(ECU|POL)-0000.";
                                        break;
                                    }

                                    //se agrega a la lista temporal si pasa las validaciones de encapsulamiento
                                //por el momento lo realizare asi ya que estoy utilizando listas no nativas, las cambiare
                                //cuando se agregue las clases correctas para las listas
                                tempPol.InsertarAlFinal(new SatelitePolar(polarId, sateliteId, nombre, frecuencia ?? ""));
                                }

                                if(!transaccionExitosa) break;
                            }
                        }
                    }

                    //------------->VALIDACIONES PARA ANTENAS TERRESTRES<-------------
                    if(transaccionExitosa)
                    {
                        XmlNodeList antenas = doc.SelectNodes("/orbitnet/antenas_terrestres/antena")!;
                        if(antenas.Count == 0)
                        {
                            transaccionExitosa = false;
                            causaFallo = "No se encontraron antenas terrestres " +
                                         "bajo '/orbitnet/antenas_terrestres/antena'.";
                        }
                        else
                        {
                            foreach(XmlNode nodo in antenas)
                            {
                                string? id = nodo.Attributes["id"]?.Value?.Trim();
                                string? nombre = nodo.SelectSingleNode("nombre")?.InnerText?.Trim();
                                string? coords = nodo.SelectSingleNode("coordenadas")?.InnerText?.Trim();
                                string? ip = nodo.SelectSingleNode("ip_nodo")?.InnerText?.Trim();

                                if(string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(nombre) ||
                                string.IsNullOrWhiteSpace(coords) || string.IsNullOrWhiteSpace(ip))
                                {
                                    transaccionExitosa = false;
                                    causaFallo = "Antena con campos vacíos o faltantes.";
                                    break;
                                }

                                //Validar coordenadas
                                if(!Regex.IsMatch(coords, PatronCoordenadas))
                                {
                                    transaccionExitosa = false;
                                    causaFallo = $"Coordenadas inválidas '{coords}' " +
                                    $"en antena '{id}'. ";
                                    break;
                                }

                                //Validar IP
                                if(!Regex.IsMatch(ip, PatronIpv4))
                                {
                                    transaccionExitosa = false;
                                    causaFallo = $"IP inválido '{ip}' en antena '{id}'.";
                                    break;
                                }

                                //Acumular en lista temporal, despues lo tengo que cambiar
                                tempAnt.InsertarAlFinal(new AntenaDato(id, nombre, coords, ip));
                            }
                        }
                    }
                }
            }
            catch (XmlException ex)
            {
                transaccionExitosa = false;
                causaFallo = $"Error de parseo Xml: {ex.Message}";
            }
            catch (Exception ex)
            {
                transaccionExitosa = false;
                causaFallo = $"Error de procesamiento: {ex.Message}";
            }

            // ── RESOLUCIÓN TRANSACCIONAL (Commit / Rollback) ─────────────────
            if (transaccionExitosa)
            {
                // COMMIT: recorrer listas temporales e insertar en listas permanentes
                // Insertar ecuatoriales
                SateliteEcuNode? actualEcu = tempEcu.ObtenerCabeza();
                while (actualEcu != null)
                {
                    baseDatosEcu.InsertarAlFinal(actualEcu.Ecuatorial);
                    actualEcu = actualEcu.Siguiente;
                }

                // Insertar polares
                SatelitePolNode? actualPol = tempPol.ObtenerCabeza();
                while (actualPol != null)
                {
                    baseDatosPol.InsertarAlFinal(actualPol.Polar);
                    actualPol = actualPol.Siguiente;
                }
                // Insertar antenas
                AntenaNode? actualAnt = tempAnt.ObtenerCabeza();
                while (actualAnt != null)
                {
                    baseDatosAnt.InsertarAlFinal(actualAnt.Antena);
                    actualAnt = actualAnt.Siguiente;
                }

                // TODO FASE 2 - Integrante 1: Reemplazar con RedSatelitalPlano
                // TODO FASE 2 - Integrante 2: Insertar en RegistroSatelites (AVL)

                string msgExito = $"Transacción completada con éxito. " +
                $"ECU: {tempEcu.Tamano}, " +
                $"POL: {tempPol.Tamano}, " +
                $"Antenas: {tempAnt.Tamano}.";

                bitacoraAuditoria.Registrar("INFO", msgExito);
                ViewBag.SuccessMessage = msgExito;
                }
                else
                {

                // ROLLBACK: algo falló, no se toca nada
                string msgFallo = $"Transacción abortada (Rollback). Causa: {causaFallo}. " +
                                   "La memoria RAM permanece intacta.";

                bitacoraAuditoria.Registrar("ERROR", msgFallo);
                ViewBag.ErrorMessage = msgFallo;
                }

                return View("Index", viewModel);
            }
            
            [HttpPost]
            public IActionResult Limpiar()
            {
                baseDatosEcu.Limpiar();
                baseDatosPol.Limpiar();
                baseDatosAnt.Limpiar();

        // TODO FASE 2 - Integrante 1: Reemplazar con redSatelital.Clear();
            // TODO FASE 2 - Integrante 2: registroAvl.Clear();

                bitacoraAuditoria.Registrar("INFO", "Se ejecutó la purga de datos de la memoria RAM. ");
                return RedirectToAction("Index");
            }

            //Limpiar logs
            [HttpPost]
            public IActionResult LimpiarLogs()
            {
                bitacoraAuditoria.Limpiar();
                bitacoraAuditoria.Registrar("INFO", "Se ejecutó la purga del historial de auditoría.");
                return RedirectToAction("Index");
            }
        }
}
