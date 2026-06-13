using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using OrbitNet.Models;
using OrbitNet.Models.TDAs;
using System;
using System.Xml;
using System.Text.RegularExpressions;

namespace OrbitNet.Controllers
{
    
    // Controlador especializado en el procesamiento, validación sintáctica
    // e ingesta atómica de archivos de configuración espacial XML.
   
    public class XmlController : Controller
    {
        // Expresiones Regulares de validación sintáctica obligatorias
        private const string PatronIdSatelite = @"^SAT-(ECU|POL)-\d{4}$";
        private const string PatronIpv4 = @"^(?:(?:25[0-5]|2[0-4]\d|[01]?\d?\d)\.){3}(?:25[0-5]|2[0-4]\d|[01]?\d?\d)$";
        private const string PatronCoordenadas = @"^-?\d{1,2}\.\d{4,6},-?\d{1,3}\.\d{4,6}$";

        [HttpPost]
        public IActionResult CargarXml(IFormFile archivoXml)
        {
            if (archivoXml == null || archivoXml.Length == 0)
            {
                TempData["ErrorMessage"] = "Por favor, seleccione un archivo XML válido.";
                return RedirectToAction("Index", "Home");
            }

            // Registramos el inicio del suceso en la bitácora global compartida
            Memoria.BitacoraAuditoria.Registrar("INFO", $"Iniciando carga de archivo: '{archivoXml.FileName}'");

            // Instancia de matriz temporal para aislamiento de fallos (Rollback)
            SparseMatrix tempMatrix = new SparseMatrix(1000);

            bool transaccionExitosa = true;
            string causaFallo = "";
            int contadorInserciones = 0;

            try
            {
                // Mitigación estricta de Vulnerabilidades XXE dictada por la cátedra
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

                    // ==========================================================
                    // 1. VALIDACIÓN Y PARSEO: SATÉLITES ECUATORIALES
                    // ==========================================================
                    XmlNodeList satelitesEcua = doc.SelectNodes("/orbitnet/constelaciones_ecuatoriales/satelite")!;
                    if (satelitesEcua.Count == 0)
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

                            //VERIFICA QUE LOS NUEVOS DATOS A INGRESAR NO EXISTA YA EN LA MEMORIA RAM
                            if (Memoria.RedSatelital.ExisteId(id) || tempMatrix.ExisteId(id))
                            {
                                transaccionExitosa = false;
                                causaFallo = $"El satélite '{id}' ya existe en la memoria RAM. " +
                                $"No se permiten datos duplicados.";
                            }

                            int fila = CalcularFila(id);
                            int columna = CalcularColumna(id);
                            tempMatrix.Insert(fila, columna, id, nombre, enlaceIp, "ECU", null);
                            contadorInserciones++;
                        }
                    }

                    // ==========================================================
                    // 2. VALIDACIÓN Y PARSEO: SATÉLITES POLARES
                    // ==========================================================
                    if (transaccionExitosa)
                    {
                        XmlNodeList polares = doc.SelectNodes("/orbitnet/orbitas_polares/polar")!;
                        if (polares.Count == 0)
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
                                    causaFallo = "Óbita polar con ID vacío o faltantes.";
                                    break;
                                }

                                XmlNodeList satelitesPol = polar.SelectNodes("child::satelite")!;

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

                                    if (!Regex.IsMatch(sateliteId, PatronIdSatelite))
                                    {
                                        transaccionExitosa = false;
                                        causaFallo = $"ID inválido '{sateliteId}' en <orbitas_polares>. Formato requerido: SAT-(ECU|POL)-0000.";
                                        break;
                                    }

                                    //VERIFICA QUE LOS NUEVOS DATOS A INGRESAR NO EXISTA YA EN LA MEMORIA RAM
                                    if (Memoria.RedSatelital.ExisteId(sateliteId) || tempMatrix.ExisteId(sateliteId))
                                    {
                                        transaccionExitosa = false;
                                        causaFallo = $"El satélite '{sateliteId}' ya existe en la memoria RAM. " +
                                        $"No se permiten datos duplicados.";
                                    }

                                    int fila = CalcularFila(sateliteId);
                                    int columna = CalcularColumna(sateliteId);
                                    tempMatrix.Insert(fila, columna, sateliteId, nombre, "0.0.0.0", "POL", frecuencia ?? "");
                                    contadorInserciones++;
                                }

                                if (!transaccionExitosa) break;
                            }
                        }
                    }

                    // ==========================================================
                    // 3. VALIDACIÓN Y PARSEO: ANTENAS TERRESTRES
                    // ==========================================================
                    if (transaccionExitosa)
                    {
                        XmlNodeList antenas = doc.SelectNodes("/orbitnet/antenas_terrestres/antena")!;
                        if (antenas.Count == 0)
                        {
                            transaccionExitosa = false;
                            causaFallo = "No se encontraron antenas terrestres bajo '/orbitnet/antenas_terrestres/antena'.";
                        }
                        else
                        {
                            foreach (XmlNode nodo in antenas)
                            {
                                string? id = nodo.Attributes?["id"]?.Value?.Trim();
                                string? nombre = nodo.SelectSingleNode("nombre")?.InnerText?.Trim();
                                string? coords = nodo.SelectSingleNode("coordenadas")?.InnerText?.Trim();
                                string? ip = nodo.SelectSingleNode("ip_nodo")?.InnerText?.Trim();

                                if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(nombre) ||
                                    string.IsNullOrWhiteSpace(coords) || string.IsNullOrWhiteSpace(ip))
                                {
                                    transaccionExitosa = false;
                                    causaFallo = "Antena con campos vacíos o faltantes.";
                                    break;
                                }

                                if (!Regex.IsMatch(coords, PatronCoordenadas))
                                {
                                    transaccionExitosa = false;
                                    causaFallo = $"Coordenadas inválidas '{coords}' en antena '{id}'.";
                                    break;
                                }

                                if (!Regex.IsMatch(ip, PatronIpv4))
                                {
                                    transaccionExitosa = false;
                                    causaFallo = $"IP inválido '{ip}' en antena '{id}'.";
                                    break;
                                }

                                //VERIFICA QUE LOS NUEVOS DATOS A INGRESAR NO EXISTA YA EN LA MEMORIA RAM
                                if (Memoria.RedSatelital.ExisteId(id) || tempMatrix.ExisteId(id))
                                {
                                    transaccionExitosa = false;
                                    causaFallo = $"El satélite '{id}' ya existe en la memoria RAM. " +
                                    $"No se permiten datos duplicados.";
                                }                                

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

            // ==========================================================
            // PROTOCOLO COMMIT / ROLLBACK (Fase Transaccional)
            // ==========================================================
            if (transaccionExitosa)
            {

                // COMMIT: Pasamos los datos del plano temporal a la Matriz Dispersa Real en la RAM
                foreach (var nodo in tempMatrix.GetAllNodes())
                {
                    if (nodo != null)
                    {
                        Memoria.RedSatelital.Insert(nodo.Row, nodo.Col, nodo.Id, nodo.Name, nodo.IpAddress, nodo.NodeType, nodo.ExtraData);
                    }
                }

                string msgExito = $"Transacción completada con éxito. Se insertaron {contadorInserciones} elementos en la matriz dispersa.";
                Memoria.BitacoraAuditoria.Registrar("INFO", msgExito);
                TempData["SuccessMessage"] = msgExito;
            }
            else
            {
                // ROLLBACK: No alteramos el almacén central y registramos la causa en la bitácora
                string msgFallo = $"Transacción abortada (Rollback). Causa: {causaFallo}. La memoria RAM permanece intacta.";
                Memoria.BitacoraAuditoria.Registrar("ERROR", msgFallo);
                TempData["ErrorMessage"] = msgFallo;
            }

            // Redireccionamos el flujo al Dashboard principal para forzar el re-renderizado
            return RedirectToAction("Index", "Home");
        }

        // ==========================================================
        // FUNCIONES AUXILIARES MATEMÁTICAS PARA COORDENADAS ORTOGONALES
        // ==========================================================
        
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
            if (id.StartsWith("SAT-ECU", StringComparison.OrdinalIgnoreCase))
                return 1 + (CalcularFila(id) % 100);
            else if (id.StartsWith("SAT-POL", StringComparison.OrdinalIgnoreCase))
                return 101 + (CalcularFila(id) % 100);
            else if (id.StartsWith("ANT", StringComparison.OrdinalIgnoreCase))
                return 201 + (CalcularFila(id) % 100);
            else
                return 1;
        }
    }
}