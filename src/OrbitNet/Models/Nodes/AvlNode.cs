namespace OrbitNet.Models.Nodes
{
    public class AvlNode
    {
        // Atributos obligatorios según el enunciado del catálogo global
        public string SatelliteId { get; set; }  // Llave única (Ej: SAT-ECU-0001) [cite: 205, 298]
        public string Name { get; set; }         // Nombre identificador [cite: 206]
        public double Frequency { get; set; }    // Frecuencia de operación [cite: 207]
        public int Height { get; set; }          // Altura del nodo para el balanceo [cite: 208]

        // Punteros lógicos autorreferenciados a los hijos izquierdo y derecho
        public AvlNode LeftChild { get; set; }   // [cite: 209]
        public AvlNode RightChild { get; set; }  // [cite: 209]

        // Constructor: inicializa el satélite listo para ser insertado
        public AvlNode(string satelliteId, string name, double frequency)
        {
            SatelliteId = satelliteId;
            Name = name;
            Frequency = frequency;
            Height = 1; // Arranca en 1 porque es una hoja nueva
            LeftChild = null;
            RightChild = null;
        }
    }
}