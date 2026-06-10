namespace OrbitNet.Models.Interfaces;
/// <summary>
/// Interfaz base para todos los TDAs del simulador OrbitNet hola mundo
/// </summary>
public interface IAbstractCollection
{
    int Count { get; }
    void Clear();
    bool IsEmpty { get; }
}