# OrbitNet-NetCore

Proyecto Único IPC2 2026 - Sistema Web de Monitoreo, Enrutamiento y Simulación de Conexiones Satelitales Distribuidas en Red Local.

---

##  Integrantes

| Nombre            | Carné     | Rol                          |
| ----------------- | --------- | ---------------------------- |
| Bryan Merida      | 202400085 | Matriz Dispersa Ortogonal    |
| Daniela Velásquez  | 202503522  | Árbol AVL                    |
| [Compañero 2]     | [Carné]   | ABB Cola de Prioridad        |
| Yosselin Oxlaj     | 202503415   | Lista Enlazada + XML + Regex |

---

#  Cómo empezar a trabajar

## 1. Clonar el repositorio

```bash
git clone https://github.com/bryancastellanos11/IPC2_Proyecto_2026_Grupo12.git
cd IPC2_Proyecto_2026_Grupo12
```

---

## 2. Crear su rama de trabajo

Cada integrante debe trabajar únicamente en su propia rama.

```bash
git checkout -b feature/tu-nombre
```

Ejemplo:

```bash
git checkout -b feature/bryan
```

---

## 3. Asignación Fase 1

| Integrante        | Módulo Principal             | Archivo Principal                           |
| ----------------- | ---------------------------- | ------------------------------------------- |
| Bryan Merida      | Matriz Dispersa Ortogonal    | src/OrbitNet.Core/TDAs/RedSatelitalPlano.cs |
| Compañero 1       | Árbol AVL                    | src/OrbitNet.Core/TDAs/RegistroSatelites.cs |
| Compañero 2       | ABB Cola de Prioridad        | src/OrbitNet.Core/TDAs/BufferMensajes.cs    |
| Yosselin Oxlaj       | Lista Enlazada + XML + Regex | src/OrbitNet.Core/TDAs/LogAuditoria.cs      |

---

## 4. Flujo de trabajo diario

Antes de comenzar a programar:

```bash
git checkout main
git pull
git checkout feature/tu-nombre
git merge main
```

---

Al finalizar los cambios:

```bash
git add .
git commit -m "feat: descripcion clara de lo realizado"
git push origin feature/tu-nombre
```

Ejemplos:

```bash
git commit -m "feat: insercion ordenada en matriz dispersa"
git commit -m "feat: rotacion simple AVL"
git commit -m "fix: correccion de busqueda ABB"
git commit -m "docs: actualizacion README"
```

---

##  Reglas del Proyecto

* No trabajar directamente sobre la rama `main`.
* Cada integrante debe utilizar su propia rama `feature`.
* Realizar commits frecuentes y descriptivos.
* Mantener el código comentado cuando sea necesario.
* Hacer `git pull` antes de comenzar a trabajar.
* Resolver conflictos antes de crear Pull Requests.
* Realizar al menos 3 commits significativos por semana.

---

#  Estructura del Proyecto

```text
IPC2_Proyecto_2026_Grupo12/
│
├── .gitignore
├── README.md
│
├── docs/
│
├── scripts/
│
├── src/
│   │
│   ├── OrbitNet.Core/
│   │   ├── Interfaces/
│   │   ├── Nodes/
│   │   ├── TDAs/
│   │   └── Validators/
│   │
│   ├── OrbitNet.Services/
│   │
│   ├── OrbitNet.Web/
│   │
│   └── OrbitNet.Tests/
│
└── IPC2_Proyecto_2026_Grupo12.sln
```

---

#  Tecnologías

* C#
* .NET 8
* ASP.NET Core MVC
* Graphviz
* xUnit / NUnit
* Git & GitHub

---

#  Estrategia de Ramas

```text
main
│
└── develop
     │
     ├── feature/bryan
     ├── feature/compi1
     ├── feature/compi2
     └── feature/compi3
```

---

#  Objetivo Fase 1

Implementar:

* Estructura inicial del proyecto. 
* Configuración Git Flow. 
* Interfaces base.
* Nodos de las estructuras.
* Carga XML mediante XPath.
* Validaciones mediante Regex.
* Pruebas iniciales de carga masiva.

```
```
