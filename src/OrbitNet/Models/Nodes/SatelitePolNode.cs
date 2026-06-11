using OrbitNet.Models.TDAs;
namespace OrbitNet.Models.Nodes
{
    public class SatelitePolNode
    {
        public SatelitePolar Polar { get; set; }
        public SatelitePolNode? Siguiente { get; set; }

        public SatelitePolNode(SatelitePolar polar)
        {
            Polar = polar;
            Siguiente = null;
        }
    }
}