using OrbitNet.Models.Interfaces;
using OrbitNet.Models.Nodes;
using System;

namespace OrbitNet.Models.TDAs;

public class SparseMatrix : IAbstractCollection
{
    // ========= CAMPOS PRIVADOS ======== 
    private readonly int _maxSatelites;     // Tamaño máximo configurable
    private MatrixNode?[] _satelites;       // Arreglo fijo de referencias en RAM
    private int _contadorSatelites;         // Contador actual manual

    private HeaderNode? _rowHeaders;
    private HeaderNode? _colHeaders;

    // ======== CONSTRUCTOR =======
    public SparseMatrix(int maxSatelites = 1000)
    {
        _maxSatelites = maxSatelites;
        _satelites = new MatrixNode?[_maxSatelites]; 
        _contadorSatelites = 0;
        _rowHeaders = null;
        _colHeaders = null;
    }

    // PROPIEDADES (Implementación obligatoria de tu interfaz de auditoría)
    public int Count => _contadorSatelites;
    public bool IsEmpty => _contadorSatelites == 0;
    public int MaxCapacity => _maxSatelites;  

    //============ MÉTODOS PÚBLICOS =========== 

    public void Insert(int row, int col, string id, string name, string ipAddress, string nodeType, string? extraData)
    {
        if (_contadorSatelites >= _maxSatelites)
            throw new InvalidOperationException($"No se pueden agregar más satélites. Máximo: {_maxSatelites}");

        if (Search(row, col) != null)
            throw new InvalidOperationException($"Ya existe satélite en ({row},{col})");

        if (!IsValidSatelliteId(id) && !id.StartsWith("ANT", StringComparison.OrdinalIgnoreCase))  
            throw new ArgumentException($"ID inválido: {id}");

        if (!IsValidIpAddress(ipAddress))
            throw new ArgumentException($"IP inválida: {ipAddress}");

        MatrixNode newNode = new MatrixNode(row, col, id, name, ipAddress, nodeType, extraData);

        InsertRowHeader(row);
        InsertColHeader(col);
        InsertInRow(newNode);
        InsertInColumn(newNode);

        _satelites[_contadorSatelites] = newNode;
        _contadorSatelites++;
    }

    public void Insert(int row, int col, string id, string name, string ipAddress, string nodeType)
    {
        Insert(row, col, id, name, ipAddress, nodeType, null);
    }

    public MatrixNode? Search(int row, int col)
    {
        HeaderNode? rowHeader = FindRowHeader(row);
        if (rowHeader == null || rowHeader.Access == null)
            return null;

        MatrixNode? current = rowHeader.Access;
        while (current != null && current.Col <= col)
        {
            if (current.Col == col)
                return current;
            current = current.Right;
        }
        return null;
    }

    public MatrixNode? SearchByIndex(int index)
    {
        if (index < 0 || index >= _contadorSatelites)
            return null;
        return _satelites[index];
    }

    public void Delete(int row, int col)
    {
        MatrixNode? nodeToDelete = Search(row, col);
        if (nodeToDelete == null)
            throw new InvalidOperationException($"No existe satélite en ({row},{col})");

        // Reconectar punteros horizontales en RAM
        if (nodeToDelete.Left != null)
            nodeToDelete.Left.Right = nodeToDelete.Right;
        else
        {
            HeaderNode? rowHeader = FindRowHeader(row);
            if (rowHeader != null)
                rowHeader.Access = nodeToDelete.Right;
        }

        if (nodeToDelete.Right != null)
            nodeToDelete.Right.Left = nodeToDelete.Left;

        // Reconectar punteros verticales en RAM
        if (nodeToDelete.Up != null)
            nodeToDelete.Up.Down = nodeToDelete.Down;
        else
        {
            HeaderNode? colHeader = FindColHeader(col);
            if (colHeader != null)
                colHeader.Access = nodeToDelete.Down;
        }

        if (nodeToDelete.Down != null)
            nodeToDelete.Down.Up = nodeToDelete.Up;

        // Eliminar del arreglo interno
        int indexToRemove = -1;
        for (int i = 0; i < _contadorSatelites; i++)
        {
            if (_satelites[i] == nodeToDelete)
            {
                indexToRemove = i;
                break;
            }
        }

        if (indexToRemove >= 0)
        {
            for (int i = indexToRemove; i < _contadorSatelites - 1; i++)
            {
                _satelites[i] = _satelites[i + 1];
            }
            _satelites[_contadorSatelites - 1] = null; 
            _contadorSatelites--;
        }
    }

    public void Clear()
    {
        _rowHeaders = null;
        _colHeaders = null;

        for (int i = 0; i < _contadorSatelites; i++)
        {
            _satelites[i] = null;
        }
        _contadorSatelites = 0;
    }

    public MatrixNode?[] GetAllSatellites()
    {
        MatrixNode?[] result = new MatrixNode?[_contadorSatelites];
        for (int i = 0; i < _contadorSatelites; i++)
        {
            result[i] = _satelites[i];
        }
        return result;
    }

    // ==========================================================
    // RECORRIDOS MANUALES REFACTORIZADOS (CERO GENERIC)
    // ==========================================================


    public MatrixNode[] GetAllNodes()
    {
        MatrixNode[] temporal = new MatrixNode[_contadorSatelites];
        int indice = 0;

        HeaderNode? rowCurrent = _rowHeaders;
        while (rowCurrent != null)
        {
            MatrixNode? nodeCurrent = rowCurrent.Access;
            while (nodeCurrent != null)
            {
                temporal[indice] = nodeCurrent;
                indice++;
                nodeCurrent = nodeCurrent.Right;
            }
            rowCurrent = rowCurrent.Next;
        }
        return temporal;
    }

    public MatrixNode[] GetNodesByType(string nodeType)
    {
        int totalTipo = CountByType(nodeType);
        MatrixNode[] resultado = new MatrixNode[totalTipo];
        int indice = 0;

        MatrixNode[] todos = GetAllNodes();
        for (int i = 0; i < todos.Length; i++)
        {
            if (todos[i].NodeType == nodeType)
            {
                resultado[indice] = todos[i];
                indice++;
            }
        }
        return resultado;
    }


    public int CountByType(string nodeType)
    {
        int count = 0;
        MatrixNode[] todos = GetAllNodes();
        for (int i = 0; i < todos.Length; i++)
        {
            if (todos[i].NodeType == nodeType)
                count++;
        }
        return count;
    }

    // ==========================================================
    // MÉTODOS PRIVADOS DE CABECERAS Y ENLACES
    // ==========================================================

    private void InsertRowHeader(int row)
    {
        if (_rowHeaders == null)
        {
            _rowHeaders = new HeaderNode(row);
            return;
        }

        HeaderNode current = _rowHeaders;
        HeaderNode? previous = null;

        while (current != null && current.Index < row)
        {
            previous = current;
            current = current.Next!;
        }

        if (current != null && current.Index == row)
            return;

        HeaderNode newHeader = new HeaderNode(row);

        if (previous == null)
        {
            newHeader.Next = _rowHeaders;
            _rowHeaders = newHeader;
        }
        else
        {
            newHeader.Next = current;
            previous.Next = newHeader;
        }
    }

    private void InsertColHeader(int col)
    {
        if (_colHeaders == null)
        {
            _colHeaders = new HeaderNode(col);
            return;
        }

        HeaderNode current = _colHeaders;
        HeaderNode? previous = null;

        while (current != null && current.Index < col)
        {
            previous = current;
            current = current.Next!;
        }

        if (current != null && current.Index == col)
            return;

        HeaderNode newHeader = new HeaderNode(col);

        if (previous == null)
        {
            newHeader.Next = _colHeaders;
            _colHeaders = newHeader;
        }
        else
        {
            newHeader.Next = current;
            previous.Next = newHeader;
        }
    }

    private void InsertInRow(MatrixNode newNode)
    {
        HeaderNode? rowHeader = FindRowHeader(newNode.Row);
        if (rowHeader == null) return;

        if (rowHeader.Access == null)
        {
            rowHeader.Access = newNode;
            return;
        }

        MatrixNode? current = rowHeader.Access;
        MatrixNode? previous = null;

        while (current != null && current.Col < newNode.Col)
        {
            previous = current;
            current = current.Right;
        }

        if (previous == null)
        {
            newNode.Right = rowHeader.Access;
            rowHeader.Access.Left = newNode;
            rowHeader.Access = newNode;
        }
        else
        {
            newNode.Right = current;
            newNode.Left = previous;
            previous.Right = newNode;
            if (current != null)
                current.Left = newNode;
        }
    }

    private void InsertInColumn(MatrixNode newNode)
    {
        HeaderNode? colHeader = FindColHeader(newNode.Col);
        if (colHeader == null) return;

        if (colHeader.Access == null)
        {
            colHeader.Access = newNode;
            return;
        }

        MatrixNode? current = colHeader.Access;
        MatrixNode? previous = null;

        while (current != null && current.Row < newNode.Row)
        {
            previous = current;
            current = current.Down;
        }

        if (previous == null)
        {
            newNode.Down = colHeader.Access;
            colHeader.Access.Up = newNode;
            colHeader.Access = newNode;
        }
        else
        {
            newNode.Down = current;
            newNode.Up = previous;
            previous.Down = newNode;
            if (current != null)
                current.Up = newNode;
        }
    }

    private HeaderNode? FindRowHeader(int row)
    {
        HeaderNode? current = _rowHeaders;
        while (current != null && current.Index != row)
            current = current.Next;
        return current;
    }

    private HeaderNode? FindColHeader(int col)
    {
        HeaderNode? current = _colHeaders;
        while (current != null && current.Index != col)
            current = current.Next;
        return current;
    }

    private bool IsValidSatelliteId(string id)
    {
        if (string.IsNullOrEmpty(id)) return false;
        return System.Text.RegularExpressions.Regex.IsMatch(id, @"^SAT-[A-Z]{3}-\d{4}$");
    }

    private bool IsValidIpAddress(string ip)
    {
        if (string.IsNullOrEmpty(ip)) return false;
        return System.Text.RegularExpressions.Regex.IsMatch(ip,
            @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");
    }
}