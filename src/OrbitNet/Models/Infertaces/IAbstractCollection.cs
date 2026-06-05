namespace OrbitNet.Models.Interfaces;

public interface IAbstractCollection
{
    int Count { get; }
    void Clear();
    bool IsEmpty { get; }
}