namespace OrbitNet.Models.Nodes;

public class HeaderNode
{
    public int Index { get; set; }
    public HeaderNode Next { get; set; }
    public MatrixNode Access { get; set; }

    public HeaderNode(int index)
    {
        Index = index;
    }
}