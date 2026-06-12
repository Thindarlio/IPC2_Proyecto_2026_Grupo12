using Microsoft.AspNetCore.Mvc;
using OrbitNet.Models; // Necesario para mandar a traer tu clase global "Memoria"

namespace OrbitNet.Controllers
{
    /// <summary>
    /// Controlador especializado en administrar las acciones y limpiezas de los registros de auditoría del sistema.
    /// </summary>
    public class LogsController : Controller
    {
        [HttpPost]
        public IActionResult LimpiarLogs()
        {
            // FUSIÓN: Llamamos al método Clear() de tu TDA LogAuditoria manual en la RAM global
            Memoria.BitacoraAuditoria.Clear();
            
            // FUSIÓN: Registramos el evento de purga usando tu método manual Registrar()
            Memoria.BitacoraAuditoria.Registrar("INFO", "Se purgó la bitácora de auditoría de forma manual.");
            
            // Guardamos el mensaje de éxito en el TempData para que sobreviva al redireccionamiento
            TempData["SuccessMessage"] = "Bitácora de auditoría reiniciada correctamente en la memoria RAM.";
            
            // Redireccionamos el flujo de vuelta al Dashboard principal para refrescar la pantalla
            return RedirectToAction("Index", "Home");
        }
    }
}