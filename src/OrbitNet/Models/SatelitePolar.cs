namespace OrbitNet.Models
{
    
    public class SatelitePolar
    {
        private string polarID = "";
        private string sateliteID;
        private string nombre = "";

        private string frecuencia;

        public SatelitePolar(string polarID, string sateliteID, string nombre, string frecuencia)
        {
            PolarID = polarID;
            this.sateliteID = sateliteID;
            Nombre = nombre;
            this.frecuencia = frecuencia;
        }

        public string PolarID
        {
            get { return polarID; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("El id del polar no puede estar vacío.");
                }
                polarID = value;
            }
        }

        public string SateliteID
        {
            get { return sateliteID; }
        }

        public string Nombre
        {
            get { return nombre; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("El nombre del satélite ecuatorial no puede estar vacío.");
                }
                nombre = value;
            }
        }

        public string Frecuencia
        {
            get { return frecuencia; }
        }

        public string ObtenerDescripcion()
        {
            return "Polar ID: " + polarID +  ", Satelite ID: " +  sateliteID + 
            ", Nombre: " + nombre + ", Frecuencia: " + frecuencia;
        }
    }
}