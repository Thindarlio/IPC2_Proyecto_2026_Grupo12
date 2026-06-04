using System;
using OrbitNet.Models.Nodes; 

namespace OrbitNet.Models.TDAs
{
    public class RegistroSatelites
    {
        // Puntero raíz del árbol almacenado en el Heap de la RAM [cite: 41, 42]
        private AvlNode? raiz;

        public RegistroSatelites()
        {
            raiz = null; // Inicialmente el catálogo global está vacío [cite: 202]
        }

        // ==========================================================
        // OPERACIONES AUXILIARES DE MEDICIÓN
        // ==========================================================

        // Devuelve la altura de un nodo de forma segura sin romper el programa si es nulo
        private int ObtenerAltura(AvlNode? nodo)
        {
            if (nodo == null) return 0;
            return nodo.Height;
        }

        // Calcula el Factor de Equilibrio con la fórmula exacta: FE = H_der - H_izq [cite: 210]
        private int ObtenerFactorEquilibrio(AvlNode nodo)
        {
            if (nodo == null) return 0;
            return ObtenerAltura(nodo.RightChild) - ObtenerAltura(nodo.LeftChild); // [cite: 210]
        }

        // Devuelve el valor mayor entre dos enteros
        private int Maximo(int a, int b)
        {
            return (a > b) ? a : b;
        }

        // ==========================================================
        // ROTACIONES MANUALES DE PUNTEROS 
        // ==========================================================

        // Rotación Simple a la Derecha (LL) - Inserción en hijo izquierdo del subárbol izquierdo [cite: 214, 217]
        private AvlNode? RotacionDerecha(AvlNode? y)
        {

            // 1. PROTECCIÓN DE SEGURIDAD: Si y es nulo, no se puede hacer nada
             if (y == null) return null;

            // 2. PROTECCIÓN DEL HIJO: Si no hay hijo izquierdo, es imposible rotar a la derecha
            if (y.LeftChild == null) return y;

            AvlNode? x = y.LeftChild;
            AvlNode? T2 = x.RightChild;

           // Quirófano de punteros: intercambiamos referencias en memoria RAM [cite: 39]
            x.RightChild = y;
            y.LeftChild = T2;

            // Recalculamos alturas de abajo hacia arriba
            y.Height = Maximo(ObtenerAltura(y.LeftChild), ObtenerAltura(y.RightChild)) + 1;
            x.Height = Maximo(ObtenerAltura(x.LeftChild), ObtenerAltura(x.RightChild)) + 1;

            return x; // Retorna la nueva raíz de este subárbol
        }

        // Rotación Simple a la Izquierda (RR) - Inserción en hijo derecho del subárbol derecho [cite: 213, 218]
        private AvlNode? RotacionIzquierda(AvlNode? x)
        {

            // 1. PROTECCIÓN DE SEGURIDAD: Si x es nulo, no se puede hacer nada
            if (x == null) return null;

             // 2. PROTECCIÓN DEL HIJO: Si no hay hijo derecho, es imposible rotar a la izquierda
            if (x.RightChild == null) return x;

            AvlNode? y = x.RightChild;
            AvlNode? T2 = y.LeftChild;

            // Quirófano de punteros: intercambiamos referencias en memoria RAM [cite: 39]
            y.LeftChild = x;
            x.RightChild = T2;

            // Recalculamos alturas de abajo hacia arriba
            x.Height = Maximo(ObtenerAltura(x.LeftChild), ObtenerAltura(x.RightChild)) + 1;
            y.Height = Maximo(ObtenerAltura(y.LeftChild), ObtenerAltura(y.RightChild)) + 1;

            return y; // Retorna la nueva raíz de este subárbol
        }

        // ==========================================================
        // MÉTODO DE INSERCIÓN Y BALANCEO
        // ==========================================================

        // Método público expuesto para ser invocado por el motor de ingesta XML [cite: 263, 270]
        public void Insertar(string id, string nombre, double frecuencia)
        {
            raiz = InsertarRecursivo(raiz, id, nombre, frecuencia);
        }

        // Algoritmo interno recursivo para ubicar la posición y balancear el nodo
        private AvlNode? InsertarRecursivo(AvlNode? nodoActual, string id, string nombre, double frecuencia)
        {
            // Base de la recursividad: se encuentra el espacio vacío en el Heap [cite: 42]
            if (nodoActual == null)
            {
                return new AvlNode(id, nombre, frecuencia);
            }

            //cite_start Comparamos las llaves alfabéticamente para decidir la dirección del tránsito [cite: 205]
            int comparacion = string.Compare(id, nodoActual.SatelliteId);

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
                // Regula que no existan duplicados en el catálogo global [cite: 205]
                throw new Exception($"El Satélite con ID '{id}' ya existe en el catálogo global.");
            }

            // Actualizamos la altura del nodo actual tras la inserción de la hoja [cite: 208]
            nodoActual.Height = 1 + Maximo(ObtenerAltura(nodoActual.LeftChild), ObtenerAltura(nodoActual.RightChild));

            // Evaluamos el factor de balanceo riguroso sobre el nodo ancestro [cite: 52, 210]
            int fe = ObtenerFactorEquilibrio(nodoActual);

            // ------------------------------------------------------
            // EVALUACIÓN DE LOS 4 CASOS DE DESBALANCE (|FE| >= 2) [cite: 211, 212]
            // ------------------------------------------------------

            // Caso 1: Rotación Simple Derecha (LL) [cite: 214, 217]
            if (fe < -1 && string.Compare(id, nodoActual.LeftChild?.SatelliteId) < 0)
            {
                return RotacionDerecha(nodoActual);
            }

            // Caso 2: Rotación Simple Izquierda (RR) [cite: 213, 218]
            if (fe > 1 && string.Compare(id, nodoActual.RightChild?.SatelliteId) > 0)
            {
                return RotacionIzquierda(nodoActual);
            }

            // Caso 3: Rotación Doble Izquierda-Derecha (LR) [cite: 215]
            if (fe < -1 && string.Compare(id, nodoActual.LeftChild?.SatelliteId) > 0)
            {
                nodoActual.LeftChild = RotacionIzquierda(nodoActual.LeftChild); // [cite: 215]
                return RotacionDerecha(nodoActual); // [cite: 215]
            }

            // Caso 4: Rotación Doble Derecha-Izquierda (RL) [cite: 216]
            if (fe > 1 && string.Compare(id, nodoActual.RightChild?.SatelliteId) < 0)
            {
                nodoActual.RightChild = RotacionDerecha(nodoActual.RightChild); // [cite: 216]
                return RotacionIzquierda(nodoActual); // [cite: 216]
            }

            return nodoActual; // El subárbol permanece equilibrado [cite: 51, 57]
        }
    }
}