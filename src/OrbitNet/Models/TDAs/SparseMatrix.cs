using OrbitNet.Models.Interfaces;
using OrbitNet.Models.Nodes;
using System;
using System.Text;

namespace OrbitNet.Models.TDAs;

public class SparseMatrix : IAbstractCollection
{
    // ========= CAMPOS PRIVADOS (SOLO CABECERAS ENLAZADAS) ========= 
    private HeaderNode? _rowHeaders;   // Cabeceras de fila
    private HeaderNode? _colHeaders;   // Cabeceras de columna
    private int _count;                // Contador manual

    // ======== CONSTRUCTOR ========
    public SparseMatrix()
    {
        _rowHeaders = null;
        _colHeaders = null;
        _count = 0;
    }

    // PROPIEDADES
    public int Count => _count;
    public bool IsEmpty => _count == 0;

    // ========== INSERCIÓN (SIN ARREGLOS) ==========
    public void Insert(int row, int col, string id, string name, string ipAddress, string nodeType, string? extraData = null)
    {
        // Validaciones
        if (Search(row, col) != null)
            throw new InvalidOperationException($"Ya existe elemento en ({row},{col})");

        if (!IsValidSatelliteId(id) && !id.StartsWith("ANT", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException($"ID inválido: {id}");

        if (!IsValidIpAddress(ipAddress))
            throw new ArgumentException($"IP inválida: {ipAddress}");

        // Crear nuevo nodo
        MatrixNode newNode = new MatrixNode(row, col, id, name, ipAddress, nodeType, extraData);

        // Obtener o crear cabeceras
        HeaderNode rowHeader = GetOrCreateRowHeader(row);
        HeaderNode colHeader = GetOrCreateColHeader(col);

        // 1. Inserción horizontal (en la fila, ordenada por columna)
        if (rowHeader.Access == null)
        {
            rowHeader.Access = newNode;
        }
        else if (col < rowHeader.Access.Col)
        {
            newNode.Right = rowHeader.Access;
            rowHeader.Access.Left = newNode;
            rowHeader.Access = newNode;
        }
        else
        {
            MatrixNode? current = rowHeader.Access;
            while (current.Right != null && current.Right.Col < col)
            {
                current = current.Right;
            }
            newNode.Right = current.Right;
            if (current.Right != null)
                current.Right.Left = newNode;
            current.Right = newNode;
            newNode.Left = current;
        }

        // 2. Inserción vertical (en la columna, ordenada por fila)
        if (colHeader.Access == null)
        {
            colHeader.Access = newNode;
        }
        else if (row < colHeader.Access.Row)
        {
            newNode.Down = colHeader.Access;
            colHeader.Access.Up = newNode;
            colHeader.Access = newNode;
        }
        else
        {
            MatrixNode? current = colHeader.Access;
            while (current.Down != null && current.Down.Row < row)
            {
                current = current.Down;
            }
            newNode.Down = current.Down;
            if (current.Down != null)
                current.Down.Up = newNode;
            current.Down = newNode;
            newNode.Up = current;
        }

        _count++;
    }

    // Sobrecarga para compatibilidad
    public void Insert(int row, int col, string id, string name, string ipAddress, string nodeType)
    {
        Insert(row, col, id, name, ipAddress, nodeType, null);
    }

    // ========== BÚSQUEDA O(r+c) ==========
    public MatrixNode? Search(int row, int col)
    {
        HeaderNode? rowHeader = FindRowHeader(row);
        if (rowHeader == null || rowHeader.Access == null)
            return null;

        MatrixNode? current = rowHeader.Access;
        while (current != null)
        {
            if (current.Col == col)
                return current;
            current = current.Right;
        }
        return null;
    }

    // ========== ELIMINACIÓN CON RECONEXIÓN QUIRÚRGICA ==========
    public void Delete(int row, int col)
    {
        HeaderNode? rowHeader = FindRowHeader(row);
        HeaderNode? colHeader = FindColHeader(col);

        if (rowHeader == null || colHeader == null) return;

        MatrixNode? target = Search(row, col);
        if (target == null) return;

        // 1. Desconectar horizontalmente
        if (rowHeader.Access == target)
        {
            rowHeader.Access = target.Right;
            if (rowHeader.Access != null)
                rowHeader.Access.Left = null;
        }
        else
        {
            if (target.Left != null)
                target.Left.Right = target.Right;
            if (target.Right != null)
                target.Right.Left = target.Left;
        }

        // 2. Desconectar verticalmente
        if (colHeader.Access == target)
        {
            colHeader.Access = target.Down;
            if (colHeader.Access != null)
                colHeader.Access.Up = null;
        }
        else
        {
            if (target.Up != null)
                target.Up.Down = target.Down;
            if (target.Down != null)
                target.Down.Up = target.Up;
        }

        // 3. Limpiar cabeceras vacías
        RemoveRowHeaderIfEmpty(rowHeader);
        RemoveColHeaderIfEmpty(colHeader);

        _count--;
    }

    // ========== LIMPIEZA TOTAL ==========
    public void Clear()
    {
        _rowHeaders = null;
        _colHeaders = null;
        _count = 0;
    }

    // ========== RECORRIDOS Ya sin Arreglos full listas enlazadas ==========
    public MatrixNode[] GetAllNodes()
    {
        MatrixNode[] result = new MatrixNode[_count];
        int index = 0;

        HeaderNode? rowCurrent = _rowHeaders;
        while (rowCurrent != null)
        {
            MatrixNode? nodeCurrent = rowCurrent.Access;
            while (nodeCurrent != null)
            {
                result[index] = nodeCurrent;
                index++;
                nodeCurrent = nodeCurrent.Right;
            }
            rowCurrent = rowCurrent.Next;
        }
        return result;
    }

    public MatrixNode[] GetNodesByType(string nodeType)
    {
        // Primero contamos cuántos hay del tipo
        int count = 0;
        HeaderNode? rowCurrent = _rowHeaders;
        while (rowCurrent != null)
        {
            MatrixNode? nodeCurrent = rowCurrent.Access;
            while (nodeCurrent != null)
            {
                if (nodeCurrent.NodeType == nodeType)
                    count++;
                nodeCurrent = nodeCurrent.Right;
            }
            rowCurrent = rowCurrent.Next;
        }

        MatrixNode[] result = new MatrixNode[count];
        int index = 0;
        rowCurrent = _rowHeaders;
        while (rowCurrent != null)
        {
            MatrixNode? nodeCurrent = rowCurrent.Access;
            while (nodeCurrent != null)
            {
                if (nodeCurrent.NodeType == nodeType)
                {
                    result[index] = nodeCurrent;
                    index++;
                }
                nodeCurrent = nodeCurrent.Right;
            }
            rowCurrent = rowCurrent.Next;
        }
        return result;
    }

    public int CountByType(string nodeType)
    {
        int count = 0;
        HeaderNode? rowCurrent = _rowHeaders;
        while (rowCurrent != null)
        {
            MatrixNode? nodeCurrent = rowCurrent.Access;
            while (nodeCurrent != null)
            {
                if (nodeCurrent.NodeType == nodeType)
                    count++;
                nodeCurrent = nodeCurrent.Right;
            }
            rowCurrent = rowCurrent.Next;
        }
        return count;
    }

    // ========== Verificación de existencia por ID ==========
    public bool ExisteId(string id)
    {
        HeaderNode? rowCurrent = _rowHeaders;
        while (rowCurrent != null)
        {
            MatrixNode? nodeCurrent = rowCurrent.Access;
            while (nodeCurrent != null)
            {
                if (nodeCurrent.Id == id)
                    return true;
                nodeCurrent = nodeCurrent.Right;
            }
            rowCurrent = rowCurrent.Next;
        }
        return false;
    }

    // ========== MÉTODOS PRIVADOS PARA CABECERAS ==========

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

    private HeaderNode GetOrCreateRowHeader(int row)
    {
        if (_rowHeaders == null)
        {
            _rowHeaders = new HeaderNode(row);
            return _rowHeaders;
        }

        if (row < _rowHeaders.Index)
        {
            HeaderNode nuevo = new HeaderNode(row) { Next = _rowHeaders };
            _rowHeaders = nuevo;
            return nuevo;
        }

        HeaderNode actual = _rowHeaders;
        while (actual.Next != null && actual.Next.Index < row)
        {
            actual = actual.Next;
        }

        if (actual.Next != null && actual.Next.Index == row)
            return actual.Next;

        if (actual.Index == row)
            return actual;

        HeaderNode nuevoNodo = new HeaderNode(row) { Next = actual.Next };
        actual.Next = nuevoNodo;
        return nuevoNodo;
    }

    private HeaderNode GetOrCreateColHeader(int col)
    {
        if (_colHeaders == null)
        {
            _colHeaders = new HeaderNode(col);
            return _colHeaders;
        }

        if (col < _colHeaders.Index)
        {
            HeaderNode nuevo = new HeaderNode(col) { Next = _colHeaders };
            _colHeaders = nuevo;
            return nuevo;
        }

        HeaderNode actual = _colHeaders;
        while (actual.Next != null && actual.Next.Index < col)
        {
            actual = actual.Next;
        }

        if (actual.Next != null && actual.Next.Index == col)
            return actual.Next;

        if (actual.Index == col)
            return actual;

        HeaderNode nuevoNodo = new HeaderNode(col) { Next = actual.Next };
        actual.Next = nuevoNodo;
        return nuevoNodo;
    }

    private void RemoveRowHeaderIfEmpty(HeaderNode rowHeader)
    {
        if (rowHeader.Access != null) return;

        if (_rowHeaders == rowHeader)
        {
            _rowHeaders = rowHeader.Next;
            return;
        }

        HeaderNode? actual = _rowHeaders;
        while (actual != null && actual.Next != rowHeader)
        {
            actual = actual.Next;
        }

        if (actual != null)
        {
            actual.Next = rowHeader.Next;
        }
    }

    private void RemoveColHeaderIfEmpty(HeaderNode colHeader)
    {
        if (colHeader.Access != null) return;

        if (_colHeaders == colHeader)
        {
            _colHeaders = colHeader.Next;
            return;
        }

        HeaderNode? actual = _colHeaders;
        while (actual != null && actual.Next != colHeader)
        {
            actual = actual.Next;
        }

        if (actual != null)
        {
            actual.Next = colHeader.Next;
        }
    }

    // ========== VALIDACIONES ==========
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