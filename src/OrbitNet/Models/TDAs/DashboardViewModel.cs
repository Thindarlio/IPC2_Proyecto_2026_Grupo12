using OrbitNet.Models;
using OrbitNet.Models.TDAs;
using OrbitNet.Models.Nodes;

namespace OrbitNet.Models
{
    public class DashboardViewModel
    {
        public SparseMatrix RedSatelital { get; set; } = null!;  
        public LogAuditoria Logs { get; set; } = null!;

    
    }
}