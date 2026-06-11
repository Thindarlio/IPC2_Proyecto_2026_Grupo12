using System.Text.RegularExpressions;
using OrbitNet.Models;
using OrbitNet.Models.Nodes;
using OrbitNet.Models.Interfaces;
namespace OrbitNet.Models.TDAs

{
    public class ListaAntenas : IAbstractCollection
    {
        private AntenaNode? Cabeza;
        private AntenaNode? Cola;
        public int Count { get; private set; }
        public bool IsEmpty => Cabeza == null;

        public ListaAntenas()
        {
            Cabeza = null;
            Cola = null;
            Count = 0;
        }

        //Inserta al final un nuevo registro
        public void InsertarAlFinal(AntenaDato dato)
        {
            AntenaNode NuevoNodo = new AntenaNode (dato);
            
            if(IsEmpty)
            {
                Cabeza = NuevoNodo;
                Cola = NuevoNodo;
            }
            else
            {
                Cola!.Siguiente = NuevoNodo;
                Cola = NuevoNodo;
            }
            Count++;
        }

        //Para recorrer en el Commit
        public AntenaNode? ObtenerCabeza()
        {
            return Cabeza;
        }
        public void Limpiar()
        {
            Cabeza = null;
            Cola = null;
            Count = 0;
        }
    }
}