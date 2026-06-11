using System.Text.RegularExpressions;
using OrbitNet.Models;
using OrbitNet.Models.Nodes;
using OrbitNet.Models.Interfaces;
namespace OrbitNet.Models.TDAs

{
    public class ListaSatelitesEcu : IAbstractCollection
    {
        private SateliteEcuNode? Cabeza;
        private SateliteEcuNode? Cola;
        public int Count { get; private set; }
        public bool IsEmpty => Cabeza == null;

        public ListaSatelitesEcu()
        {
            Cabeza = null;
            Cola = null;
            Count = 0;
        }

        //Inserta al final un nuevo registro
        public void InsertarAlFinal(SateliteEcu sateEcu)
        {
            SateliteEcuNode NuevoNodo = new SateliteEcuNode (sateEcu);
            
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
        public SateliteEcuNode? ObtenerCabeza()
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