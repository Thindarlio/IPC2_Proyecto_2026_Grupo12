//Lista
using System.Text.RegularExpressions;
using OrbitNet.Models;
using OrbitNet.Models.Nodes;
using OrbitNet.Models.Infertaces;
namespace OrbitNet.Models.TDAs
{
    public class LogAuditoria : IAbstractCollection
    {
        private LogNode? Cabeza;
        private LogNode? Cola;
        public int Tamano { get; private set; }
        public bool EstaVacia => Cabeza == null;

        public LogAuditoria()
        {
            Cabeza = null;
            Cola = null;
            Tamano = 0;
        }

        //Inserta al final un nuevo registro
        public void InsertarAlFinal(LogDato log)
        {
            LogNode NuevoNodo = new LogNode(log);
            
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

        public void Registrar(string tipo, string mensaje)
        {
            LogDato nuevoLog = new LogDato(tipo, mensaje);
            InsertarAlFinal(nuevoLog);
        }

        //Filtra logs por expresión regular
        public string SearchLogRegex(string pattern)
        {
            string resultado = "";
            LogNode? actual = Cabeza;

            while (actual != null)
            {
                if(Regex.IsMatch(actual.Dato.Mensaje, pattern))
                {
                    resultado += $"[{actual.Dato.FechaHora}]" +
                    $"{actual.Dato.Tipo}: " +
                    $"{actual.Dato.Mensaje}\n";
                }
                actual = actual.Siguiente;
            }
            return resultado;
        }

        public LogNode? ObtenerCabeza()
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