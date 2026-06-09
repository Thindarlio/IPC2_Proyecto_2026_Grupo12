using System.Text.RegularExpressions;
using OrbitNet.Models;
using OrbitNet.Models.Nodes;
using OrbitNet.Models.Infertaces;
namespace OrbitNet.Models.TDAs

{
    public class ListaAntenas : IAbstractCollection
    {
        private AntenaNode? Cabeza;
        private AntenaNode? Cola;
        public int Tamano { get; private set; }
        public bool EstaVacia => Cabeza == null;

        public ListaAntenas()
        {
            Cabeza = null;
            Cola = null;
            Tamano = 0;
        }

        //Inserta al final un nuevo registro
        public void InsertarAlFinal(AntenaDato dato)
        {
            AntenaNode NuevoNodo = new AntenaNode (dato);
            
            if(EstaVacia)
            {
                Cabeza = NuevoNodo;
                Cola = NuevoNodo;
            }
            else
            {
                Cola!.Siguiente = NuevoNodo;
                Cola = NuevoNodo;
            }
            Tamano++;
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
            Tamano = 0;
        }
    }
}