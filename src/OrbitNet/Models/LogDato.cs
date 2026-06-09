namespace OrbitNet.Models
{
    
    public class LogDato
    {
        public DateTime FechaHora { get; }
        public string Tipo { get; } //INFO, ALERTA, ERROR

        public string Mensaje { get; }

        public LogDato(string tipo, string mensaje)
        {
            FechaHora = DateTime.Now;
            Tipo = tipo;
            Mensaje = mensaje;
        }

        public string ObtenerDescripcion()
        {
            string tag = Tipo == "INFO" ? "OK" : "FAIL";
            return $"{FechaHora} | {Tipo,-5} ({tag}) | {Mensaje}";
        }
    }
}