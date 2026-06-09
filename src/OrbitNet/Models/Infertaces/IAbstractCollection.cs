using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
namespace OrbitNet.Models.Infertaces;
// Firma base abstracta que debe implementar todo TDA para control de auditoría
public interface IAbstractCollection
{
    int Tamano { get; } 
    void Limpiar(); 
    bool EstaVacia { get; } 
}