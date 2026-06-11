namespace OrbitNet.Models
{
    /// <summary>
    /// Modelo estándar de ASP.NET Core MVC para la gestión y despliegue de excepciones en el cliente web.
    /// </summary>
    public class ErrorViewModel
    {
        // Almacena el identificador único de la petición HTTP que falló
        public string? RequestId { get; set; }

        // Propiedad booleana que decide si se muestra o no el ID en la vista HTML
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}