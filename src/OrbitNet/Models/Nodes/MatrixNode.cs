namespace OrbitNet.Models.Nodes;

public class MatrixNode
{
    public int Row { get; set; }
    public int Col { get; set; }
    public string Id { get; set; }
    public string IpAddress { get; set; }
    public MatrixNode Up { get; set; }
    public MatrixNode Down { get; set; }
    public MatrixNode Left { get; set; }
    public MatrixNode Right { get; set; }

    public MatrixNode(int row, int col, string id, string ipAddress)
    {
        Row = row;
        Col = col;
        Id = id;
        IpAddress = ipAddress;
    }
}