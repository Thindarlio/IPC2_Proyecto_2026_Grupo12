#include <iostream>
#include <string>
using namespace std;

// ============================================
// ESTRUCTURA DEL NODO (para lista enlazada)
// ============================================
struct Documento {
    string nombre;
    int paginas;
    Documento* siguiente;
};

// ============================================
// CLASE COLA DE IMPRESION (TAD)
// ============================================
class ColaImpresion {
private:
    Documento* frente;   // Primer elemento (proximo a imprimir)
    Documento* final;    // Ultimo elemento (ultimo en llegar)

public:
    // Constructor: inicializa cola vacia
    ColaImpresion() {
        frente = NULL;
        final = NULL;
    }

    // Destructor: libera memoria al finalizar
    ~ColaImpresion() {
        while (!estaVacia()) {
            procesarDocumento();
        }
    }

    // Verificar si la cola esta vacia
    bool estaVacia() {
        return (frente == NULL);
    }

    // ========== OPERACION 1: Agregar documento (Enqueue) ==========
    void agregarDocumento(string nombre, int paginas) {
        // Crear nuevo nodo
        Documento* nuevo = new Documento();
        nuevo->nombre = nombre;
        nuevo->paginas = paginas;
        nuevo->siguiente = NULL;

        // Si la cola esta vacia, el nuevo es el primero y el ultimo
        if (estaVacia()) {
            frente = nuevo;
            final = nuevo;
        } else {
            // Si no, se agrega al final
            final->siguiente = nuevo;
            final = nuevo;
        }
        cout << "\n[OK] Documento \"" << nombre << "\" agregado a la cola (" << paginas << " paginas)" << endl;
    }

    // ========== OPERACION 2: Procesar documento (Dequeue) ==========
    void procesarDocumento() {
        if (estaVacia()) {
            cout << "\n[WARNING] No hay documentos en la cola para procesar." << endl;
            return;
        }

        // Guardar el documento que vamos a eliminar
        Documento* temp = frente;
        string nombreProcesado = temp->nombre;
        int paginasProcesadas = temp->paginas;

        // Mover el frente al siguiente nodo
        frente = frente->siguiente;

        // Si la cola quedo vacia, el final tambien debe ser NULL
        if (frente == NULL) {
            final = NULL;
        }

        // Liberar memoria del nodo procesado
        delete temp;

        cout << "\n[IMPRIMIENDO] \"" << nombreProcesado << "\" (" << paginasProcesadas << " paginas)... COMPLETADO!" << endl;
    }

    // ========== OPERACION 3: Mostrar documentos pendientes ==========
    void mostrarPendientes() {
        if (estaVacia()) {
            cout << "\n[VACIA] Cola de impresion VACIA. No hay documentos pendientes." << endl;
            return;
        }

        cout << "\n--- DOCUMENTOS PENDIENTES (" << obtenerTamano() << " documentos):" << endl;
        cout << "=========================================" << endl;
        
        Documento* actual = frente;
        int posicion = 1;
        
        while (actual != NULL) {
            cout << posicion << ". \"" << actual->nombre << "\" - " << actual->paginas << " paginas" << endl;
            actual = actual->siguiente;
            posicion++;
        }
        cout << "=========================================" << endl;
    }

    // ========== OPERACION 4: Indicar siguiente documento a imprimir ==========
    void mostrarSiguiente() {
        if (estaVacia()) {
            cout << "\n[ERROR] No hay documentos pendientes. La cola esta vacia." << endl;
            return;
        }
        cout << "\n[SIGUIENTE] PROXIMO DOCUMENTO A IMPRIMIR: \"" << frente->nombre 
             << "\" (" << frente->paginas << " paginas)" << endl;
    }

    // Metodo auxiliar: obtener cantidad de documentos pendientes
    int obtenerTamano() {
        int contador = 0;
        Documento* actual = frente;
        while (actual != NULL) {
            contador++;
            actual = actual->siguiente;
        }
        return contador;
    }
};

// ============================================
// FUNCION PRINCIPAL (MENU INTERACTIVO)
// ============================================
int main() {
    ColaImpresion cola;
    int opcion;
    string nombre;
    int paginas;

    cout << "=====================================" << endl;
    cout << "   SIMULADOR DE COLA DE IMPRESION    " << endl;
    cout << "=====================================" << endl;

    do {
        cout << "\n--- MENU PRINCIPAL ---" << endl;
        cout << "1. Agregar documento" << endl;
        cout << "2. Procesar documento (imprimir)" << endl;
        cout << "3. Mostrar documentos pendientes" << endl;
        cout << "4. Mostrar proximo documento" << endl;
        cout << "5. Salir" << endl;
        cout << "Opcion: ";
        cin >> opcion;
        cin.ignore(); // Limpiar el buffer despues de leer el numero

        switch (opcion) {
            case 1:
                cout << "\n--- NUEVO DOCUMENTO ---" << endl;
                cout << "Nombre del documento: ";
                getline(cin, nombre);
                cout << "Numero de paginas: ";
                cin >> paginas;
                cin.ignore();
                cola.agregarDocumento(nombre, paginas);
                break;

            case 2:
                cola.procesarDocumento();
                break;

            case 3:
                cola.mostrarPendientes();
                break;

            case 4:
                cola.mostrarSiguiente();
                break;

            case 5:
                cout << "\n[FIN] Saliendo del simulador. Hasta luego!" << endl;
                break;

            default:
                cout << "\n[ERROR] Opcion invalida. Intente nuevamente." << endl;
        }
    } while (opcion != 5);

    return 0;
}