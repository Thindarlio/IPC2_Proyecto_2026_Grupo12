using Microsoft.AspNetCore.Mvc;
using OrbitNet.Models; // Para mandar a llamar a tu clase centralizada "Memoria"

namespace OrbitNet.Controllers
{
    /// <summary>
    /// Controlador encargado de gestionar el flujo transaccional y la administración
    /// de la Bitácora de Auditoría, implementada mediante un TDA dinámico lineal.
    /// </summary>
    public class LogsController : Controller
    {
        /// <summary>
        /// Acción POST que ejecuta la purga completa del TDA dinámico de auditoría.
        /// </summary>
        [HttpPost]
        public IActionResult LimpiarLogs()
        {
            // 1. ESTRUCTURA PRINCIPAL: Vaciamos los nodos de tu lista enlazada/cola manual (.Clear())
            // No usamos colecciones nativas; se limpian los punteros cabeza/cola directamente en RAM.
            Memoria.BitacoraAuditoria.Clear();
            
            // 2. INSERCIÓN DINÁMICA: Registramos el evento creando un nuevo nodo dinámico en el TDA
            Memoria.BitacoraAuditoria.Registrar("INFO", "Se purgó de forma manual y lineal la bitácora de auditoría.");
            
            // 3. MENSAJE TRANSACCIONAL: Guardamos la alerta en el TempData para el salto entre controladores
            TempData["SuccessMessage"] = "La estructura dinámica de la bitácora ha sido reiniciada correctamente.";
            
            // 4. REDIRECCIÓN: Volvemos al Dashboard principal para que renderice el nuevo nodo único
            return RedirectToAction("Index", "Home");
        }
    }
}