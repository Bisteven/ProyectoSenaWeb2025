# Sistema de Matrícula - Proyecto SENA 2025

Sistema web desarrollado en C# ASP.NET Core para la gestión de matrículas con almacenamiento en formato JSON.

## Características

- ✅ Crear nuevas matrículas
- ✅ Listar todas las matrículas
- ✅ Editar matrículas existentes
- ✅ Eliminar matrículas
- ✅ Almacenamiento de datos en formato JSON
- ✅ Interfaz web moderna con Bootstrap 5

## Requisitos

- .NET 8.0 SDK o superior
- Visual Studio 2022, Visual Studio Code, o cualquier editor compatible

## Instalación y Ejecución

1. Abrir una terminal en la carpeta del proyecto
2. Restaurar las dependencias (si es necesario):
   ```bash
   dotnet restore
   ```
3. Ejecutar la aplicación:
   ```bash
   dotnet run
   ```
4. Abrir el navegador en: `https://localhost:5001` o `http://localhost:5000`

## Estructura del Proyecto

```
ProyectoSena2025/
├── Controllers/          # Controladores MVC
│   └── HomeController.cs
├── Models/              # Modelos de datos
│   └── Matricula.cs
├── Services/            # Servicios de negocio
│   ├── IMatriculaService.cs
│   └── MatriculaService.cs
├── Views/               # Vistas Razor
│   ├── Home/
│   │   ├── Index.cshtml
│   │   ├── Crear.cshtml
│   │   └── Editar.cshtml
│   └── Shared/
│       ├── _Layout.cshtml
│       └── _ValidationScriptsPartial.cshtml
├── wwwroot/             # Archivos estáticos
│   ├── css/
│   └── js/
├── Data/                # Almacenamiento JSON (se crea automáticamente)
│   └── matriculas.json
└── Program.cs            # Punto de entrada de la aplicación
```

## Datos de Matrícula

El sistema almacena los siguientes datos:
- ID (generado automáticamente)
- Número de Documento
- Nombre Completo
- Programa
- Ficha
- Fecha de Matrícula
- Estado (Activa, Inactiva, Cancelada)

## Almacenamiento JSON

Los datos se guardan en el archivo `Data/matriculas.json` en formato JSON. Este archivo se crea automáticamente cuando se guarda la primera matrícula.

## Tecnologías Utilizadas

- ASP.NET Core 8.0
- C# 12
- Razor Pages / MVC
- Bootstrap 5
- jQuery
- System.Text.Json

