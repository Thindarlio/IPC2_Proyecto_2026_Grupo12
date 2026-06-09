namespace OrbitNet.Models.Nodes
{
    public class AntenaNode
    {
        public AntenaDato Antena { get; set; }
        public AntenaNode? Siguiente { get; set; }

        public AntenaNode(AntenaDato antena)
        {
            Antena = antena;
            Siguiente = null;
        }
    }
}
