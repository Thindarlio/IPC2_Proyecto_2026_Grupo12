using System.Text.RegularExpressions;
using OrbitNet.Models;
using OrbitNet.Models.Nodes;
using OrbitNet.Models.Interfaces;
namespace OrbitNet.Models.TDAs

{
    public class ListaSatelitesPol : IAbstractCollection
    {
        private SatelitePolNode? Cabeza;
        private SatelitePolNode? Cola;
        public int Count { get; private set; }
        public bool IsEmpty => Cabeza == null;

        public ListaSatelitesPol()
        {
            Cabeza = null;
            Cola = null;
            Count = 0;
        }

        //Inserta al final un nuevo registro
        public void InsertarAlFinal(SatelitePolar polar)
        {
            SatelitePolNode NuevoNodo = new SatelitePolNode (polar);
            
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
        public SatelitePolNode? ObtenerCabeza()
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