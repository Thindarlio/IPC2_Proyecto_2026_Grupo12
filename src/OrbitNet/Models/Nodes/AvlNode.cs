using System;

namespace OrbitNet.Models.Nodes
{
   
    public class AvlNode
    {
        // ==========================================================
        // ATRIBUTOS DE DATOS (Sección 5.2 del Enunciado)
        // ==========================================================
        public string SatelliteId { get; set; }  // Llave única de búsqueda (Ej: SAT-ECU-0001)
        public string Name { get; set; }         // Nombre identificador (Ej: Starlink-Norte-A)
        public double Frequency { get; set; }    // Frecuencia de operación lógica
        public int Height { get; set; }          // Altura del nodo en el subárbol para balanceo

        // ==========================================================
        // REQUERIMIENTOS DE RED Y PERSISTENCIA COMPLEMENTARIOS
        // ==========================================================
        public string IpAddress { get; set; }   // Dirección IPv4 para enrutamiento cross-port
        
        // Aquí se conectará tu Cola de Prioridad interna de mensajes (TDA ABB) más adelante
        // Por ahora lo dejamos como 'object?' para que compile sin pedir el archivo del ABB
        public object? Buffer { get; set; }

        // ==========================================================
        // PUNTEROS LÓGICOS AUTORREFERENCIADOS
        // ==========================================================
        public AvlNode? LeftChild { get; set; }   // Puntero físico al hijo izquierdo en el Heap
        public AvlNode? RightChild { get; set; }  // Puntero físico al hijo derecho en el Heap

        // ==========================================================
        // CONSTRUCTOR COMPLETO PARA INGESTA MASIVA
        // ==========================================================
        public AvlNode(string satelliteId, string name, double frequency, string ipAddress = "127.0.0.1")
        {
            SatelliteId = satelliteId;
            Name = name;
            Frequency = frequency;
            IpAddress = ipAddress;
            Height = 1;      // Todo nodo nuevo se inserta inicialmente como una hoja (Altura = 1)
            Buffer = null;   // El buffer de procesamiento arranca limpio y vacío en RAM
            LeftChild = null;
            RightChild = null;
        }
    }
}
