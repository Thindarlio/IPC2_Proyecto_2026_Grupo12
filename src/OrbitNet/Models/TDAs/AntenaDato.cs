namespace OrbitNet.Models
{
    
    public class AntenaDato
    {
        private string idAntena;
        private string nombre = "";
        private string coordenadas = "";

        private string ipNodo = "";

        public AntenaDato(string idAntena, string nombre, string coordenadas, string ipNodo)
        {
            this.idAntena = idAntena;
            Nombre = nombre;
            Coordenadas = coordenadas;
            IpNodo = ipNodo;
        }

        public string IdAntena
        {
            get { return idAntena; }
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

        public string Coordenadas
        {
            get { return coordenadas; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Las coordenadas del satélite polar no puede estar vacío.");
                }
                coordenadas = value;
            }
        }

        public string IpNodo
        {
            get { return ipNodo; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("El Ip del nodo del satélite polar no puede estar vacío.");
                }
                ipNodo = value;
            }
        }

        public string ObtenerDescripcion()
        {
            return "Antena ID: " + idAntena +  ", Nombre: " +  nombre + 
            ", Coordenadas: " + coordenadas + ", IP Nodo: " + ipNodo;
        }
    }
}