using OrbitNet.Models.TDAs;

namespace OrbitNet.Models.Nodes
{
    public class LogNode
    {
        public LogDato Dato { get; set; }
        public LogNode? Siguiente { get; set; }

        public LogNode(LogDato log)
        {
            Dato = log;
            Siguiente = null;
        }
    }
}