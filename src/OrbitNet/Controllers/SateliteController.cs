using Microsoft.AspNetCore.Mvc;
using OrbitNet.Models; // Para llamar a "Memoria"
using System;

namespace OrbitNet.Controllers
{
    public class SateliteController : Controller
    {
        /// <summary>
        /// Acción POST adaptada a las firmas reales del TDA SparseMatrix de Daniela.
        /// </summary>
        [HttpPost]
        public IActionResult InsertarSatelite(int longitud, int latitud, string nombre)
        {
            if (string.IsNullOrEmpty(nombre))
            {
                TempData["ErrorMessage"] = "El nombre del satélite no puede estar vacío.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                // 1. FORMATEO DE DATOS OBLIGATORIOS PARA TU MATRIZ:
                // Generamos dinámicamente un ID que cumpla con tu expresión regular: ^SAT-[A-Z]{3}-\d{4}$
                string idSimulado = $"SAT-ORB-{new Random().Next(1000, 9999)}";
                
                // Generamos una IP válida que pase tu Regex de red
                string ipSimulada = $"192.168.{new Random().Next(1, 254)}.{new Random().Next(1, 254)}";
                
                // Definimos el tipo de nodo por defecto para la red satelital
                string tipoNodo = "Satelite";

                // 2. LLAMADA CORRECRTA AL TDA: Usamos 'Insert' en inglés con las variables que tu código pide
                // Fila (latitud), Columna (longitud), id, name, ip, type
                Memoria.RedSatelital.Insert(latitud, longitud, idSimulado, nombre, ipSimulada, tipoNodo);

                // Auditoría dinámica
                Memoria.BitacoraAuditoria.Registrar(
                    "SUCCESS", 
                    $"Satélite '{nombre}' puesto en órbita en la posición ortogonal ({latitud}, {longitud}) con IP {ipSimulada}."
                );

                TempData["SuccessMessage"] = $"El satélite '{nombre}' ha sido mapeado con éxito en la red.";
            }
            catch (Exception ex)
            {
                Memoria.BitacoraAuditoria.Registrar("ERROR", $"Fallo en inserción ortogonal: {ex.Message}");
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult LimpiarMatriz()
        {
            // Tu método Clear() está perfecto, este se queda igual
            Memoria.RedSatelital.Clear();
            Memoria.BitacoraAuditoria.Registrar("WARNING", "Se ejecutó una purga total de los nodos de la matriz.");
            TempData["SuccessMessage"] = "La red de satélites ha sido reiniciada por completo.";
            return RedirectToAction("Index", "Home");
        }
    }
}