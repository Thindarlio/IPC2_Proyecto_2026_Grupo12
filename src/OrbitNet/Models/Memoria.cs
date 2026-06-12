using OrbitNet.Models.TDAs;

namespace OrbitNet.Models
{
    /// <summary>
    /// Administrador centralizado de la persistencia volátil en el Heap de la memoria RAM.
    /// Encapsula las estructuras manuales del simulador en un único circuito estático global.
    /// </summary>
    public static class Memoria
    {
        // Tus estructuras manuales puras con acceso global y sincronizado
        public static SparseMatrix RedSatelital { get; } = new SparseMatrix(1000);
        public static LogAuditoria BitacoraAuditoria { get; } = new LogAuditoria();
        public static RegistroSatelites CatalogoSatelites { get; } = new RegistroSatelites();
    }
}