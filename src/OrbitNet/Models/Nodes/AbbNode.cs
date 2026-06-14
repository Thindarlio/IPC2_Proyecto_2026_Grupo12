namespace OrbitNet.Models.Nodes;

public class AbbNode {

public string HexCode;
    public string EmisorId;
    public string DestinoIp;
    public int Prioridad;
    public string Contenido;

    public AbbNode Izquierda;
    public AbbNode Derecha;


    public AbbNode(
        string hexCode,
        string emisorId,
        string destinoIp,
        int prioridad,
        string contenido)
    {
        HexCode = hexCode;
        EmisorId = emisorId;
        DestinoIp = destinoIp;
        Prioridad = prioridad;
        Contenido = contenido;

        Izquierda = null;
        Derecha = null;
    }

}