using OrbitNet.Models.Nodes;


namespace OrbitNet.Models.TDAs;


public class BufferMensajes
{

    private AbbNode raiz;
    private int cantidad;


    public BufferMensajes()
    {
        raiz = null;
        cantidad = 0;

    }



    public void Enqueue(AbbNode nuevoMensaje)
    {
        raiz = Insertar(raiz, nuevoMensaje);
        cantidad++;

    }




    public AbbNode Dequeue()
{

    if(raiz == null)
    {
        return null;
    }


    AbbNode padre = null;
    AbbNode actual = raiz;


    // Buscar el nodo más a la derecha
    while(actual.Derecha != null)
    {
        padre = actual;
        actual = actual.Derecha;
    }


    // Si la raíz era el único nodo
    if(padre == null)
    {
        raiz = raiz.Izquierda;
    }
    else
    {
        padre.Derecha = actual.Izquierda;
    }

    cantidad--;
    return actual;
}






    private AbbNode Insertar(AbbNode actual, AbbNode nuevoMensaje)
    {

        if(actual == null)
        {
            return nuevoMensaje;
        }


        if(nuevoMensaje.Prioridad < actual.Prioridad)
        {
            actual.Izquierda =
            Insertar(actual.Izquierda, nuevoMensaje);
        }
        else
        {
            actual.Derecha =
            Insertar(actual.Derecha, nuevoMensaje);
        }


        return actual;
    }

    
public void Mostrar()
{
    Recorrer(raiz);
}


public int Count
{
    get
    {
        return cantidad;
    }
}


public bool IsEmpty
{
    get
    {
        return raiz == null;
    }
}


public void Clear()
{
    raiz = null;
    cantidad = 0;
}

private void Recorrer(AbbNode nodo)
{
    if(nodo == null)
        return;

    Recorrer(nodo.Izquierda);

    Console.WriteLine(
        "Codigo: " + nodo.HexCode +
        " Prioridad: " + nodo.Prioridad
    );

    Recorrer(nodo.Derecha);
}


public AbbNode Search(string hexCode)
{
    return Buscar(raiz, hexCode);
}

private AbbNode Buscar(AbbNode actual, string hexCode)
{

    if(actual == null)
    {
        return null;
    }


    if(actual.HexCode == hexCode)
    {
        return actual;
    }


    AbbNode encontrado = Buscar(actual.Izquierda, hexCode);


    if(encontrado != null)
    {
        return encontrado;
    }


    return Buscar(actual.Derecha, hexCode);
}
}
