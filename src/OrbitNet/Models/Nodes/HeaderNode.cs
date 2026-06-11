using System;

namespace OrbitNet.Models.Nodes
{
    
    public class HeaderNode
    {
        // El índice posicional (puede ser la coordenada X, Y o el ID alfanumérico)
        public int Index { get; set; }

        public HeaderNode? Next { get; set; }

        public MatrixNode? Access { get; set; }

        // Constructor: Inicializa la cabecera con su coordenada base
        public HeaderNode(int index)
        {
            Index = index;
            Next = null;   
            Access = null; 
        }
    }
}