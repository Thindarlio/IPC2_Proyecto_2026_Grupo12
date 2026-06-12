using System;
using OrbitNet.Models.Nodes;
using OrbitNet.Models.TDAs;

namespace OrbitNet.Models
{
    /// <summary>
    /// Modelo de vista para el Dashboard principal.
    /// Transporta las estructuras globales y los arreglos ya filtrados hacia la interfaz HTML.
    /// </summary>
    public class DashboardViewModel
    {
        // Tus propiedades originales para la matriz y la bitácora
        public SparseMatrix RedSatelital { get; set; } = null!;
        public LogAuditoria Logs { get; set; } = null!;

        // REPARACIÓN: Agregamos los tres moldes de arreglos independientes que le hacían falta al compilador
        public MatrixNode[] SatelitesEcuatoriales { get; set; } = Array.Empty<MatrixNode>();
        public MatrixNode[] SatelitesPolares { get; set; } = Array.Empty<MatrixNode>();
        public MatrixNode[] AntenasTerrestres { get; set; } = Array.Empty<MatrixNode>();
    }
}
