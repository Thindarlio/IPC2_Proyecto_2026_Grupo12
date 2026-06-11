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
        // CAMBIO: Reemplazo de 3 listas por Matriz Dispersa
        private static readonly SparseMatrix redSatelital = new SparseMatrix(1000);

        //Expresiones Regulares Oficiales
        private const string PatronIdSatelite = @"^SAT-(ECU|POL)-\d{4}$";
        private const string PatronIpv4 = @"^(?:(?:25[0-5]|2[0-4]\d|[01]?\d?\d)\.){3}(?:25[0-5]|2[0-4]\d|[01]?\d?\d)$";
        private const string PatronCoordenadas = @"^-?\d{1,2}\.\d{4,6},-?\d{1,3}\.\d{4,6}$";

        [HttpGet]
        public IActionResult Index()
        {
            //Inicializació de logs básicos de arranque si la bitácora está vacía

            if(bitacoraAuditoria.IsEmpty)
            {
                bitacoraAuditoria.Registrar("INFO", "Sistema de simulación espacial inicializado correctamente");
                bitacoraAuditoria.Registrar("INFO", "Esperando archivo XML de configuración...");
            }

            // CAMBIO: ViewModel ahora usa SparseMatrix en lugar de 3 listas
            var viewModel = new DashboardViewModel
            {
                RedSatelital = redSatelital,
                Logs = bitacoraAuditoria
            };
            return View(viewModel);
        }

        //-----> CARGAR XML <----------
        [HttpPost]
        public IActionResult CargarXml(IFormFile archivoXml)
        {
            // CAMBIO: ViewModel usa SparseMatrix
            var viewModel = new DashboardViewModel
            {
                RedSatelital = redSatelital,
                Logs = bitacoraAuditoria
            };

            if(archivoXml == null || archivoXml.Length == 0)
            {
                ViewBag.ErrorMessage = "Por favor, seleccione un archivo XML válido.";
                return View("Index", viewModel);
            }

            bitacoraAuditoria.Registrar("INFO", $"Iniciando carga de archivo: '{archivoXml.FileName}'");

            // CAMBIO: Matriz temporal para transacción atómica (reemplaza tempEcu, tempPol, tempAnt)
            SparseMatrix tempMatrix = new SparseMatrix(1000);

            bool transaccionExitosa = true;
            string causaFallo = "";
            int contadorInserciones = 0;

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

                    // Validación Satélites Ecuatoriales

                    XmlNodeList satelitesEcua = doc.SelectNodes("/orbitnet/constelaciones_ecuatoriales/satelite")!;
                    if(satelitesEcua.Count == 0)
                    {
                        transaccionExitosa = false;
                        causaFallo = "No se encontraron nodos de satélites bajo el XPath '/orbitnet/constelaciones_ecuatoriales/satelite'.";
                    }
                    else
                    {
                        foreach (XmlNode nodo in satelitesEcua)
                        {
                            string? id = nodo.Attributes?["id"]?.Value?.Trim();
                            string? nombre = nodo.SelectSingleNode("nombre")?.InnerText?.Trim();
                            string? enlaceIp = nodo.SelectSingleNode("enlace_ip")?.InnerText?.Trim();

                            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(enlaceIp))
                            {
                                transaccionExitosa = false;
                                causaFallo = "Satélite ecuatorial con campos vacíos o faltantes.";
                                break;
                            }

                            if (!Regex.IsMatch(id, PatronIdSatelite))
                            {
                                transaccionExitosa = false;
                                causaFallo = $"ID inválido '{id}' en <constelaciones_ecuatoriales>. Formato requerido: SAT-(ECU|POL)-0000.";
                                break;
                            }

                            if (!Regex.IsMatch(enlaceIp, PatronIpv4))
                            {
                                transaccionExitosa = false;
                                causaFallo = $"El satélite [{id}] contiene una dirección IP inválida: '{enlaceIp}'.";
                                break;
                            }

                            // CAMBIO: Insertar en matriz temporal en lugar de tempEcu
                            int fila = CalcularFila(id);
                            int columna = CalcularColumna(id);
                            tempMatrix.Insert(fila, columna, id, nombre, enlaceIp, "ECU", null);
                            contadorInserciones++;
                        }
                    }

                    // Validación Satélites Polares
                    if(transaccionExitosa)
                    {
                        XmlNodeList polares = doc.SelectNodes("/orbitnet/orbitas_polares/polar")!;
                        if(polares.Count == 0)
                        {
                            transaccionExitosa = false;
                            causaFallo = "No se encontraron órbitas polares bajo '/orbitnet/orbitas_polares/polar'.";
                        }
                        else
                        {
                            foreach (XmlNode polar in polares)
                            {
                                string? polarId = polar.Attributes?["id"]?.Value?.Trim();

                               
                                if (string.IsNullOrWhiteSpace(polarId))
                                {
                                    transaccionExitosa = false;
                                    causaFallo = "Órbita polar con ID vacío o faltante.";
                                    break;
                                }

                                XmlNodeList satelitesPol = polar.SelectNodes("child::satelite")!;

                                 bitacoraAuditoria.Registrar("INFO",
                                $"DEBUG POLAR → polarId='{polarId}' satelites encontrados={satelitesPol.Count}");
                                foreach (XmlNode nodo in satelitesPol)
                                {
                                     bitacoraAuditoria.Registrar("INFO",
                                    $"DEBUG NODO → tipo='{nodo.NodeType}' nombre='{nodo.Name}' " +
                                    $"atributos={nodo.Attributes?.Count ?? 0} " +
                                    $"xml='{nodo.OuterXml}'");
                                    string? sateliteId = nodo.Attributes["id"]?.Value?.Trim();
                                    string? nombre = nodo.SelectSingleNode("nombre")?.InnerText?.Trim();
                                    string? frecuencia = nodo.SelectSingleNode("frecuencia")?.InnerText?.Trim();
                                     // ← Agrega esto temporalmente para ver qué lee
                                    bitacoraAuditoria.Registrar("INFO", 
                                    $"DEBUG POL → sateliteId='{sateliteId}' nombre='{nombre}' frecuencia='{frecuencia}'");


                                    if (string.IsNullOrWhiteSpace(sateliteId) || string.IsNullOrWhiteSpace(nombre))
                                    {
                                        transaccionExitosa = false;
                                        causaFallo = "Satélite polar con campos vacíos o faltantes.";
                                        break;
                                    }

                                    if (!Regex.IsMatch(sateliteId, PatronIdSatelite))
                                    {
                                        transaccionExitosa = false;
                                        causaFallo = $"ID inválido '{sateliteId}' en <orbitas_polares>. Formato requerido: SAT-(ECU|POL)-0000.";
                                        break;
                                    }

                                    // CAMBIO: Insertar en matriz temporal en lugar de tempPol
                                    int fila = CalcularFila(sateliteId);
                                    int columna = CalcularColumna(sateliteId);
                                    tempMatrix.Insert(fila, columna, sateliteId, nombre, "0.0.0.0", "POL", frecuencia ?? "");
                                    contadorInserciones++;
                                }

                                if(!transaccionExitosa) break;
                            }
                        }
                    }

                    // Validación Antenas Terrestres
                    if(transaccionExitosa)
                    {
                        XmlNodeList antenas = doc.SelectNodes("/orbitnet/antenas_terrestres/antena")!;
                        if(antenas.Count == 0)
                        {
                            transaccionExitosa = false;
                            causaFallo = "No se encontraron antenas terrestres bajo '/orbitnet/antenas_terrestres/antena'.";
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

                                if(!Regex.IsMatch(coords, PatronCoordenadas))
                                {
                                    transaccionExitosa = false;
                                    causaFallo = $"Coordenadas inválidas '{coords}' en antena '{id}'.";
                                    break;
                                }

                                if(!Regex.IsMatch(ip, PatronIpv4))
                                {
                                    transaccionExitosa = false;
                                    causaFallo = $"IP inválido '{ip}' en antena '{id}'.";
                                    break;
                                }

                                // CAMBIO: Insertar en matriz temporal en lugar de tempAnt
                                int fila = CalcularFila(id);
                                int columna = CalcularColumna(id);
                                tempMatrix.Insert(fila, columna, id, nombre, ip, "ANT", coords);
                                contadorInserciones++;
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

            // COMMIT / ROLLBACK
            if (transaccionExitosa)
            {
                // CAMBIO: Copiar matriz temporal a matriz permanente en lugar de las 3 listas
                foreach (var nodo in tempMatrix.GetAllNodes())
                {
                    redSatelital.Insert(nodo.Row, nodo.Col, nodo.Id, nodo.Name, nodo.IpAddress, nodo.NodeType, nodo.ExtraData);
                }

                string msgExito = $"Transacción completada con éxito. Se insertaron {contadorInserciones} elementos en la matriz dispersa.";
                bitacoraAuditoria.Registrar("INFO", msgExito);
                ViewBag.SuccessMessage = msgExito;
            }
            else
            {
                string msgFallo = $"Transacción abortada (Rollback). Causa: {causaFallo}. La memoria RAM permanece intacta.";
                bitacoraAuditoria.Registrar("ERROR", msgFallo);
                ViewBag.ErrorMessage = msgFallo;
            }

            return View("Index", viewModel);
        }
        
        [HttpPost]
        public IActionResult Limpiar()
        {
            // CAMBIO: Limpiar matriz en lugar de las 3 listas
            redSatelital.Clear();
            bitacoraAuditoria.Registrar("INFO", "Se ejecutó la purga de datos de la memoria RAM.");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult LimpiarLogs()
        {
            bitacoraAuditoria.Clear();
            bitacoraAuditoria.Registrar("INFO", "Se ejecutó la purga del historial de auditoría.");
            return RedirectToAction("Index");
        }

        // CAMBIO: Funciones auxiliares para asignar coordenadas
        private int CalcularFila(string id)
        {
            var match = Regex.Match(id, @"\d+");
            if (match.Success)
            {
                return int.Parse(match.Value);
            }
            return Math.Abs(id.GetHashCode() % 1000);
        }

        private int CalcularColumna(string id)
        {
            if (id.StartsWith("SAT-ECU"))
                return 1 + (CalcularFila(id) % 100);
            else if (id.StartsWith("SAT-POL"))
                return 101 + (CalcularFila(id) % 100);
            else if (id.StartsWith("ANT"))
                return 201 + (CalcularFila(id) % 100);
            else
                return 1;
        }
    }
}