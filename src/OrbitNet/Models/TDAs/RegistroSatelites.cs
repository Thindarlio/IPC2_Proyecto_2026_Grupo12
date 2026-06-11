using System;
using OrbitNet.Models.Nodes;

namespace OrbitNet.Models.TDAs
{
    
    public class RegistroSatelites
    {
        // Puntero raíz del árbol que apunta al primer nodo en el Heap de la RAM [cite: 42]
        private AvlNode? raiz;
        private int conteo;

        public RegistroSatelites()
        {
            raiz = null; // Inicialmente el árbol apunta a la nada (null)
            conteo = 0;
        }

        // Propiedades de estado básicas controladas manualmente
        public int Conteo => conteo;
        public bool EstaVacio => raiz == null;

        public void Limpiar()
        {
            raiz = null;
            conteo = 0;
        }

        // ==========================================================
        // OPERACIONES AUXILIARES DE MEDICIÓN (PUNTEROS PUROS)
        // ==========================================================

        private int ObtenerAltura(AvlNode? nodo)
        {
            if (nodo == null) return 0;
            return nodo.Height; // Retorna la altura calculada en el subárbol [cite: 208]
        }

        private int ObtenerFactorEquilibrio(AvlNode? nodo)
        {
            if (nodo == null) return 0;
            // Fórmula estricta dictada por el enunciado: FE = H_der - H_izq [cite: 210]
            return ObtenerAltura(nodo.RightChild) - ObtenerAltura(nodo.LeftChild);
        }

        private int Maximo(int a, int b)
        {
            return (a > b) ? a : b;
        }

        // ==========================================================
        // ROTACIONES MANUALES DE REFERENCIAS EN MEMORIA RAM [cite: 212]
        // ==========================================================

        // Rotación Simple a la Derecha (Caso LL) [cite: 214, 217]
        private AvlNode? RotacionDerecha(AvlNode? y)
        {
            if (y == null) return null;
            if (y.LeftChild == null) return y;

            AvlNode? x = y.LeftChild;
            AvlNode? T2 = x.RightChild;

            // Quirófano de punteros: Reconfiguración atómica de enlaces físicos en el Heap [cite: 42, 150]
            x.RightChild = y;
            y.LeftChild = T2;

            // Recalculamos las alturas de los nodos modificados
            y.Height = Maximo(ObtenerAltura(y.LeftChild), ObtenerAltura(y.RightChild)) + 1;
            x.Height = Maximo(ObtenerAltura(x.LeftChild), ObtenerAltura(x.RightChild)) + 1;

            return x; // Retorna el puntero de la nueva raíz de la rama
        }

        // Rotación Simple a la Izquierda (Caso RR) [cite: 213, 219]
        private AvlNode? RotacionIzquierda(AvlNode? x)
        {
            if (x == null) return null;
            if (x.RightChild == null) return x;

            AvlNode? y = x.RightChild;
            AvlNode? T2 = y.LeftChild;

            // Quirófano de punteros: Reconfiguración atómica de enlaces físicos en el Heap [cite: 42, 150]
            y.LeftChild = x;
            x.RightChild = T2;

            // Recalculamos las alturas de los nodos modificados
            x.Height = Maximo(ObtenerAltura(x.LeftChild), ObtenerAltura(x.RightChild)) + 1;
            y.Height = Maximo(ObtenerAltura(y.LeftChild), ObtenerAltura(y.RightChild)) + 1;

            return y; // Retorna el puntero de la nueva raíz de la rama
        }

        // ==========================================================
        // OPERACIÓN PRINCIPAL: INSERCIÓN RECURSIVA AVL
        // ==========================================================

        public void Insertar(string id, string nombre, double frecuencia)
        {
            if (string.IsNullOrEmpty(id)) return;
            raiz = InsertarRecursivo(raiz, id, nombre, frecuencia);
        }

        private AvlNode? InsertarRecursivo(AvlNode? nodoActual, string id, string nombre, double frecuencia)
        {
           // Si el puntero alcanza la nada, creamos el nuevo nodo de forma dinámica [cite: 42]
            if (nodoActual == null)
            {
                conteo++;
                return new AvlNode(id, nombre, frecuencia);
            }

            // Comparación de llaves alfabéticas únicas [cite: 205]
            int comparacion = string.Compare(id, nodoActual.SatelliteId, StringComparison.OrdinalIgnoreCase);

            if (comparacion < 0)
            {
                nodoActual.LeftChild = InsertarRecursivo(nodoActual.LeftChild, id, nombre, frecuencia);
            }
            else if (comparacion > 0)
            {
                nodoActual.RightChild = InsertarRecursivo(nodoActual.RightChild, id, nombre, frecuencia);
            }
            else
            {
                // Previene colisión o duplicados en el catálogo global [cite: 198, 202]
                throw new Exception($"El Satélite con ID '{id}' ya existe en el catálogo global.");
            }

            // Actualización de alturas al regresar de la recursividad
            nodoActual.Height = 1 + Maximo(ObtenerAltura(nodoActual.LeftChild), ObtenerAltura(nodoActual.RightChild));
            
            // Evaluación del factor de balanceo riguroso [cite: 52, 210]
            int fe = ObtenerFactorEquilibrio(nodoActual);

            // --- CASOS DE RE-BALANCEO MEDIANTE ROTACIONES MANUALES ---

           // Caso LL: Desbalance Izquierdo Puro [cite: 214, 217]
            if (fe < -1 && string.Compare(id, nodoActual.LeftChild?.SatelliteId, StringComparison.OrdinalIgnoreCase) < 0)
            {
                return RotacionDerecha(nodoActual);
            }

            // Caso RR: Desbalance Derecho Puro [cite: 213, 219]
            if (fe > 1 && string.Compare(id, nodoActual.RightChild?.SatelliteId, StringComparison.OrdinalIgnoreCase) > 0)
            {
                return RotacionIzquierda(nodoActual);
            }

            // Caso LR: Zig-Zag Izquierda-Derecha (Rotación doble) [cite: 215]
            if (fe < -1 && string.Compare(id, nodoActual.LeftChild?.SatelliteId, StringComparison.OrdinalIgnoreCase) > 0)
            {
                nodoActual.LeftChild = RotacionIzquierda(nodoActual.LeftChild);
                return RotacionDerecha(nodoActual);
            }

            // Caso RL: Zig-Zag Derecha-Izquierda (Rotación doble) [cite: 216]
            if (fe > 1 && string.Compare(id, nodoActual.RightChild?.SatelliteId, StringComparison.OrdinalIgnoreCase) < 0)
            {
                nodoActual.RightChild = RotacionDerecha(nodoActual.RightChild);
                return RotacionIzquierda(nodoActual);
            }

            return nodoActual; // El subárbol permanece equilibrado [cite: 52]
        }

        // ==========================================================
        // OPERACIÓN PRINCIPAL: BÚSQUEDA EN TIEMPO O(log n) [cite: 203]
        // ==========================================================

        public AvlNode? Buscar(string id)
        {
            return BuscarRecursivo(raiz, id);
        }

        private AvlNode? BuscarRecursivo(AvlNode? nodo, string id)
        {
            if (nodo == null) return null;

            int comparacion = string.Compare(id, nodo.SatelliteId, StringComparison.OrdinalIgnoreCase);

            if (comparacion == 0) return nodo; // Retorna la referencia física del objeto encontrado

            if (comparacion < 0)
            {
                return BuscarRecursivo(nodo.LeftChild, id); // Búsqueda binaria por la izquierda
            }
            
            return BuscarRecursivo(nodo.RightChild, id); // Búsqueda binaria por la derecha
        }
    }
}
