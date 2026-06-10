# OrbitNet-NetCore

Proyecto Único IPC2 2026 - Sistema Web de Monitoreo, Enrutamiento y Simulación de Conexiones Satelitales Distribuidas en Red Local.

---

# Integrantes

| Nombre            | Carné     |
| ----------------- | --------- |
| Bryan Merida      | 202400085 |
| Daniela Velásquez | 202503522 |
| Nestor Reina      | 202403978 |
| Yosselin Oxlaj    | 202503415 |

---

# Cómo empezar a trabajar

## 1. Clonar el repositorio

```bash
git clone https://github.com/bryancastellanos11/IPC2_Proyecto_2026_Grupo12.git
cd IPC2_Proyecto_2026_Grupo12
```

---

## 2. Crear tu rama de trabajo

Cada integrante debe trabajar únicamente en su propia rama.

```bash
git checkout -b feature/tu-nombre
```

Ejemplo:

```bash
git checkout -b feature/bryan
```

---

# Flujo de Trabajo

## 1. Antes de programar

Siempre verificar si existen cambios nuevos en la rama `develop`.

```bash
git checkout develop
git pull origin develop

git checkout feature/bryan
git merge develop
```

Esto garantiza que todos trabajen sobre la versión más reciente del proyecto.

---

## 2. Programar y subir cambios a Feature

Realizar los cambios correspondientes y luego:

```bash
git add .
git commit -m "feat: descripción de lo que hiciste"
git push origin feature/bryan
```

Ejemplo:

```bash
git commit -m "feat: inserción de nodos AVL"
git commit -m "feat: generación de reporte SVG"
git commit -m "fix: corrección búsqueda ABB"
```

---

## 3. Verificar cambios del equipo

Antes de continuar trabajando, revisar siempre la rama `develop` para comprobar si algún compañero ya integró cambios.

```bash
git checkout develop
git pull origin develop
```

Posteriormente:

```bash
git checkout feature/bryan
git merge develop
```

---

## 4. Integrar cambios a Develop

Una vez que los cambios fueron probados y verificados:

```bash
git checkout develop

git pull origin develop

git merge feature/bryan

git push origin develop

git checkout feature/bryan
```
# Asignación Fase 1

| Integrante        | Trabajo                      |
| ----------------- | ---------------------------- |
| Bryan Merida      | Matriz Dispersa Ortogonal    |
| Daniela Velásquez | Árbol AVL                    |
| Nestor Reina      | ABB Cola de Prioridad        |
| Yosselin Oxlaj    | Lista Enlazada + XML + Regex |

---

# Fase 2 - Asignación de Trabajo

| Integrante        | Trabajo                                                                                  |
| ----------------- | ---------------------------------------------------------------------------------------- |
| Bryan Merida      | Configuración de puertos (5000/5001), Vista Index.cshtml, GraphvizService y reportes SVG |
| Daniela Velásquez | Completar Árbol AVL (Buscar, Count, Clear, Eliminar) y apoyo en pruebas y reportes       |
| Nestor Reina      | Implementación de ABB Cola de Prioridad y AbbNode                                        |
| Yosselin Oxlaj    | Integración de LogAuditoria con la vista                                                 |

---

# Reglas del Proyecto

* No trabajar directamente sobre `main`.
* Todos los cambios deben pasar por `develop`.
* Cada integrante debe trabajar en su propia rama `feature`.
* Realizar commits descriptivos.
* Hacer pull de `develop` antes de comenzar a programar.
* Resolver conflictos antes de integrar cambios.
* Verificar que el proyecto compile antes de hacer merge a `develop`.

---

# Estructura Actual del Proyecto

```text
IPC2_Proyecto_2026_Grupo12/

│
├── docs/
│   ├── MANUAL_TECNICO.md
│   └── MANUAL_USUARIO.md
│
├── src/
│   └── OrbitNet/
│       │
│       ├── Controllers/
│       │   └── HomeController.cs
│       │
│       ├── Models/
│       │   ├── Interfaces/
│       │   │   └── IAbstractCollection.cs
│       │   │
│       │   ├── Nodes/
│       │   │   ├── MatrixNode.cs
│       │   │   ├── HeaderNode.cs
│       │   │   ├── AvlNode.cs
│       │   │   ├── AbbNode.cs
│       │   │   └── LogNode.cs
│       │   │
│       │   ├── TDAs/
│       │   │   ├── SparseMatrix.cs
│       │   │   ├── RegistroSatelites.cs
│       │   │   ├── BufferMensajes.cs
│       │   │   └── LogAuditoria.cs
│       │   │
│       │   └── ViewModels/
│       │       └── DashboardViewModel.cs
│       │
│       ├── Services/
│       │   └── GraphvizService.cs
│       │
│       ├── Views/
│       │   └── Home/
│       │       └── Index.cshtml
│       │
│       └── wwwroot/
│
└── tests/
    └── OrbitNet.Tests/
```

---

# Estrategia de Ramas

```text
main
│
└── develop
     │
     ├── feature/bryan
     ├── feature/daniela
     ├── feature/nestor
     └── feature/yosselin
```
