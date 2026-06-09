using OrbitNet.Models;
using OrbitNet.Models.TDAs;
using OrbitNet.Models.Nodes;

namespace OrbitNet.Models
{
    public class DashboardViewModel
    {
        public ListaSatelitesEcu SatelitesEcu { get; set; } = null!;
        public ListaSatelitesPol SatelitesPol { get; set; } = null!;
        public ListaAntenas      Antenas      { get; set; } = null!;
        public LogAuditoria Logs { get; set; } = null!;

        // TODO FASE 2 - Integrante 1: Reemplazar las 3 listas con RedSatelitalPlano
        // public RedSatelitalPlano RedSatelital { get; set; } = null!;
    }
}