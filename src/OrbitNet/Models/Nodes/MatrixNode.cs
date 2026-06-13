namespace OrbitNet.Models.Nodes;

public class MatrixNode
{
    // ========== COORDENADAS ==========
    public int Row { get; set; }
    public int Col { get; set; }

    // ========== DATOS (compatible con SateliteEcu, SatelitePolar, AntenaDato) ==========
    public string Id { get; set; }        // SAT-ECU-0001, SAT-POL-2001, ANT-ARG-501
    public string Name { get; set; }      // Nombre del satélite/antena
    public string IpAddress { get; set; } // Dirección IPv4
    public string NodeType { get; set; }  // "ECU", "POL", "ANT"
    public string? ExtraData { get; set; } // Frecuencia para polares, coordenadas para antenas

    // ========== PUNTEROS ORTOGONALES ==========
    public MatrixNode? Up { get; set; }
    public MatrixNode? Down { get; set; }
    public MatrixNode? Left { get; set; }
    public MatrixNode? Right { get; set; }

    // Constructor completo
    public MatrixNode(int row, int col, string id, string name, string ipAddress, string nodeType = "ECU", string? extraData = null)
    {
        Row = row;
        Col = col;
        Id = id;
        Name = name;
        IpAddress = ipAddress;
        NodeType = nodeType;
        ExtraData = extraData;
    }
}