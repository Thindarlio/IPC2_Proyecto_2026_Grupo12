namespace OrbitNet.Models.Nodes
{
    public class SateliteEcuNode
    {
        public SateliteEcu Ecuatorial { get; set; }
        public SateliteEcuNode? Siguiente { get; set; }

        public SateliteEcuNode(SateliteEcu ecuatorial)
        {
            Ecuatorial = ecuatorial;
            Siguiente = null;
        }
    }
}
