using Microsoft.AspNetCore.Mvc;
using OrbitNet.Models;

namespace OrbitNet.Controllers
{
    /// <summary>
    /// Controlador exclusivo para la gestión de la interfaz gráfica y vistas HTML del Dashboard.
    /// Consume de forma segura los datos centralizados en la clase original "Memoria".
    /// </summary>
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            // Inicialización de logs básicos si la bitácora compartida en RAM está vacía
            if (Memoria.BitacoraAuditoria.IsEmpty)
            {
                Memoria.BitacoraAuditoria.Registrar("INFO", "Sistema de simulación espacial inicializado correctamente");
                Memoria.BitacoraAuditoria.Registrar("INFO", "Esperando archivo XML de configuración...");
            }

            // Armamos el ViewModel distribuyendo la información filtrada directamente de la Memoria global
            var viewModel = new DashboardViewModel
            {
                RedSatelital = Memoria.RedSatelital,
                Logs = Memoria.BitacoraAuditoria,
                SatelitesEcuatoriales = Memoria.RedSatelital.GetNodesByType("ECU"),
                SatelitesPolares = Memoria.RedSatelital.GetNodesByType("POL"),
                AntenasTerrestres = Memoria.RedSatelital.GetNodesByType("ANT")
            };

            return View(viewModel);
        }
    }
}