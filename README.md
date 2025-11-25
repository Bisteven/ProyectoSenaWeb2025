# 📚 Sistema de Matrícula SENA 2025
## Creadores del proyecto: andres narvaez mejia , brhian estiven cataño

## 📄 Portada

**Proyecto:** Sistema de Gestión de Matrículas SENA  
**Programa:** Análisis y Desarrollo de Software  
**Versión:** 1.0.0  
**Año:** 2025  
**Tecnologías base:** ASP.NET Core 8.0, C# 12, MVC  
**Licencia:** Uso académico

---

## 📖 Introducción

El **Sistema de Matrícula SENA 2025** es una aplicación web MVC que centraliza el registro, consulta y actualización de matrículas de aprendices. Toda la información se persiste en archivos JSON dentro del proyecto y, adicionalmente, se generan comprobantes en PDF usando QuestPDF. El objetivo es entregar una solución ligera que pueda ejecutarse sin una base de datos relacional, ideal para entornos académicos.

---

## 📑 Índice

1. [Requerimientos del sistema](#1-requerimientos-del-sistema)  
2. [Instalación y ejecución](#2-instalación-y-ejecución)  
3. [Estructura del proyecto](#3-estructura-del-proyecto)  
4. [Arquitectura general](#4-arquitectura-general)  
5. [Documentación del código](#5-documentación-del-código)  
   - [5.1 Configuración](#51-configuración)  
   - [5.2 Modelo de dominio](#52-modelo-de-dominio)  
   - [5.3 Servicios](#53-servicios)  
   - [5.4 Controlador](#54-controlador)  
   - [5.5 Vistas](#55-vistas)  
6. [Flujo funcional](#6-flujo-funcional)  
7. [Tecnologías utilizadas](#7-tecnologías-utilizadas)  
8. [Estructura de datos JSON](#8-estructura-de-datos-json)  
9. [Posibles mejoras](#9-posibles-mejoras)  

---

## 1. Requerimientos del sistema

- **SDK:** .NET 8.0 o superior  
- **SO:** Windows 10/11, Linux o macOS  
- **Editor sugerido:** Visual Studio 2022 / VS Code  
- **Navegador moderno:** Edge, Chrome o Firefox  

---

## 2. Instalación y ejecución

```bash
git clone <url-repositorio>
cd ProyectoSenaWeb2025
dotnet restore
dotnet run
```

Abrir en `https://localhost:5001` (HTTPS) o `http://localhost:5000` (HTTP).

Directorio de datos generado automáticamente al guardar la primera matrícula:
- `Data/matriculas.json`
- `Data/Matriculas/matricula_{id}.json`
- `Data/Matriculas/matricula_{id}.pdf`

---

## 3. Estructura del proyecto

```
.
├── Controllers/
│   └── HomeController.cs
├── Models/
│   └── Matricula.cs
├── Services/
│   ├── IMatriculaService.cs
│   └── MatriculaService.cs
├── Views/
│   ├── Home/
│   │   ├── Index.cshtml
│   │   ├── Crear.cshtml
│   │   ├── Editar.cshtml
│   │   └── Error.cshtml
│   └── Shared/
│       ├── _Layout.cshtml
│       ├── _ViewStart.cshtml
│       ├── _ViewImports.cshtml
│       └── _ValidationScriptsPartial.cshtml
├── wwwroot/
│   ├── css/site.css
│   └── js/site.js
├── Data/
│   └── matriculas.json + subcarpeta Matriculas/
├── Program.cs
├── ProyectoSena2025.csproj
└── appsettings*.json
```

---

## 4. Arquitectura general

El proyecto sigue **MVC**: las vistas envían solicitudes al controlador `HomeController`, que delega la lógica al servicio `MatriculaService` y manipula el modelo `Matricula`. La persistencia se realiza en archivos JSON locales, sin motor de base de datos.

```
Usuario → Vista (Razor) → HomeController → MatriculaService → Archivos JSON/PDF
```

---

## 5. Documentación del código

> Cada apartado indica la ruta exacta dentro del repositorio para ubicar el archivo descrito.

### 5.1 Configuración

**Archivo:** `ProyectoSena2025.csproj`  

```1:14:ProyectoSena2025.csproj
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="QuestPDF" Version="2025.7.4" />
  </ItemGroup>

</Project>
```

- Define el framework objetivo (.NET 8.0) y agrega la dependencia `QuestPDF` usada para generar los comprobantes PDF.

**Archivo:** `Program.cs`

```1:29:Program.cs
using ProyectoSena2025.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IMatriculaService, MatriculaService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
```

- Registra MVC y el servicio `MatriculaService` como dependencia `Scoped`.  
- Configura la tubería HTTP, habilitando HTTPS y archivos estáticos (`wwwroot/`).

### 5.2 Modelo de dominio

**Archivo:** `Models/Matricula.cs`

```1:32:Models/Matricula.cs
public class Matricula
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El número de documento es obligatorio")]
    public string? NumeroDocumento { get; set; }

    [Required(ErrorMessage = "El nombre completo es obligatorio")]
    public string? NombreCompleto { get; set; }

    [Required(ErrorMessage = "El programa es obligatorio")]
    public string? Programa { get; set; }

    [Required(ErrorMessage = "La ficha es obligatoria")]
    public string? Ficha { get; set; }

    [DataType(DataType.Date)]
    public DateTime FechaMatricula { get; set; }

    public string? Estado { get; set; }
}
```

- El modelo define los campos requeridos para cada matrícula y las reglas de validación que consumen las vistas `Crear` y `Editar`.

### 5.3 Servicios

**Archivo:** `Services/IMatriculaService.cs`

```1:12:Services/IMatriculaService.cs
public interface IMatriculaService
{
    Task<List<Matricula>> ObtenerMatriculasAsync();
    Task<Matricula?> ObtenerMatriculaPorIdAsync(int id);
    Task<bool> GuardarMatriculaAsync(Matricula matricula);
    Task<bool> EliminarMatriculaAsync(int id);
}
```

- Define las operaciones CRUD asíncronas que el controlador consume.

**Archivo:** `Services/MatriculaService.cs`

```14:103:Services/MatriculaService.cs
public MatriculaService(IWebHostEnvironment environment)
{
    _environment = environment;
    _jsonFilePath = Path.Combine(_environment.ContentRootPath, "Data", "matriculas.json");
    var directory = Path.GetDirectoryName(_jsonFilePath);
    if (!Directory.Exists(directory))
    {
        Directory.CreateDirectory(directory!);
    }
}

public async Task<bool> GuardarMatriculaAsync(Matricula matricula)
{
    var matriculas = await ObtenerMatriculasAsync();
    if (matricula.Id == 0)
    {
        matricula.Id = matriculas.Count > 0 ? matriculas.Max(m => m.Id) + 1 : 1;
        matricula.FechaMatricula = DateTime.Now;
        matricula.Estado = matricula.Estado ?? "Activa";
        matriculas.Add(matricula);
    }
    else
    {
        var index = matriculas.FindIndex(m => m.Id == matricula.Id);
        if (index >= 0)
        {
            matriculas[index] = matricula;
        }
        else
        {
            return false;
        }
    }

    var options = new JsonSerializerOptions
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    var json = JsonSerializer.Serialize(matriculas, options);
    await File.WriteAllTextAsync(_jsonFilePath, json);

    var matriculaDirectory = Path.Combine(_environment.ContentRootPath, "Data", "Matriculas");
    Directory.CreateDirectory(matriculaDirectory);

    var matriculaJsonPath = Path.Combine(matriculaDirectory, $"matricula_{matricula.Id}.json");
    var matriculaJson = JsonSerializer.Serialize(matricula, options);
    await File.WriteAllTextAsync(matriculaJsonPath, matriculaJson);

    var matriculaPdfPath = Path.Combine(matriculaDirectory, $"matricula_{matricula.Id}.pdf");
    await GenerarPdfMatriculaAsync(matricula, matriculaPdfPath);

    return true;
}
```

- Controla la persistencia en `Data/matriculas.json`, crea archivos por matrícula y delega la generación de PDF a `QuestPDF`.

### 5.4 Controlador

**Archivo:** `Controllers/HomeController.cs`

```18:119:Controllers/HomeController.cs
public class HomeController : Controller
{
    private readonly IMatriculaService _matriculaService;
    private readonly IWebHostEnvironment _environment;

    public HomeController(IMatriculaService matriculaService, IWebHostEnvironment environment)
    {
        _matriculaService = matriculaService;
        _environment = environment;
    }

    public async Task<IActionResult> Index()
    {
        var matriculas = await _matriculaService.ObtenerMatriculasAsync();
        return View(matriculas);
    }

    [HttpGet]
    public IActionResult Crear()
    {
        return View(new Matricula());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(Matricula matricula)
    {
        if (ModelState.IsValid)
        {
            await _matriculaService.GuardarMatriculaAsync(matricula);
            return RedirectToAction(nameof(Index));
        }
        return View(matricula);
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var matricula = await _matriculaService.ObtenerMatriculaPorIdAsync(id);
        if (matricula == null)
        {
            return NotFound();
        }
        return View(matricula);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(Matricula matricula)
    {
        if (ModelState.IsValid)
        {
            await _matriculaService.GuardarMatriculaAsync(matricula);
            return RedirectToAction(nameof(Index));
        }
        return View(matricula);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _matriculaService.EliminarMatriculaAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> DescargarJson(int id)
    {
        var matricula = await _matriculaService.ObtenerMatriculaPorIdAsync(id);
        if (matricula == null)
        {
            return NotFound();
        }

        var filePath = Path.Combine(_environment.ContentRootPath, "Data", "Matriculas", $"matricula_{id}.json");
        if (!System.IO.File.Exists(filePath))
        {
            await _matriculaService.GuardarMatriculaAsync(matricula);
        }

        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
        return File(fileBytes, "application/json", $"matricula_{id}.json");
    }

    [HttpGet]
    public async Task<IActionResult> DescargarPdf(int id)
    {
        var matricula = await _matriculaService.ObtenerMatriculaPorIdAsync(id);
        if (matricula == null)
        {
            return NotFound();
        }

        var filePath = Path.Combine(_environment.ContentRootPath, "Data", "Matriculas", $"matricula_{id}.pdf");
        if (!System.IO.File.Exists(filePath))
        {
            await _matriculaService.GuardarMatriculaAsync(matricula);
        }

        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
        return File(fileBytes, "application/pdf", $"matricula_{id}.pdf");
    }
}
```

- Expone todas las acciones (listar, crear, editar, eliminar, descargar) y referencia al servicio para la lógica de negocio.

### 5.5 Vistas

**Vista principal:** `Views/Home/Index.cshtml`

```1:75:Views/Home/Index.cshtml
<table class="table table-striped table-hover">
    <thead class="table-dark">
        <tr>
            <th>ID</th>
            <th>Número Documento</th>
            <th>Nombre Completo</th>
            <th>Programa</th>
            <th>Ficha</th>
            <th>Fecha Matrícula</th>
            <th>Estado</th>
            <th>Acciones</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var matricula in Model)
        {
            <tr>
                <td>@matricula.Id</td>
                <td>@matricula.NumeroDocumento</td>
                <td>@matricula.NombreCompleto</td>
                <td>@matricula.Programa</td>
                <td>@matricula.Ficha</td>
                <td>@matricula.FechaMatricula.ToString("dd/MM/yyyy")</td>
                <td>
                    <span class="badge bg-@(matricula.Estado == "Activa" ? "success" : "secondary")">
                        @matricula.Estado
                    </span>
                </td>
                <td>
                    <!-- Botones Editar / JSON / PDF / Eliminar -->
                </td>
            </tr>
        }
    </tbody>
</table>
```

- Muestra la tabla de matrículas y las acciones disponibles por registro.

**Formularios:** `Views/Home/Crear.cshtml` y `Views/Home/Editar.cshtml`

- Ambos formularios comparten estructura y validaciones utilizando `asp-for` para enlazar con el modelo `Matricula`.

**Layout compartido:** `Views/Shared/_Layout.cshtml`

```1:63:Views/Shared/_Layout.cshtml
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.0/font/bootstrap-icons.css" />
<link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
...
<nav class="navbar navbar-expand-sm navbar-dark custom-navbar">
    <a class="navbar-brand fw-bold" asp-controller="Home" asp-action="Index">
        <i class="bi bi-mortarboard-fill me-2"></i>Sistema de Matrícula SENA
    </a>
</nav>
```

- Define la navegación principal, la carga de estilos y los scripts que comparten todas las vistas.

---

## 6. Flujo funcional

1. **Crear matrícula:** el usuario completa `Crear.cshtml`, el controlador valida y `MatriculaService` guarda/crea archivos.  
2. **Listar:** `Home/Index` recupera datos del JSON principal.  
3. **Editar:** vista precargada con el modelo; al enviarse, reutiliza `GuardarMatriculaAsync`.  
4. **Eliminar:** elimina la entrada y borra sus archivos individuales.  
5. **Descargar JSON/PDF:** si el archivo no existe, se regenera antes de retornarse al usuario.

---

## 7. Tecnologías utilizadas

- ASP.NET Core MVC 8.0  
- C# 12  
- Razor Views  
- Bootstrap 5.3 + Bootstrap Icons  
- jQuery 3.6  
- QuestPDF 2025.7.4  
- System.Text.Json

---

## 8. Estructura de datos JSON

```json
{
  "Id": 1,
  "NumeroDocumento": "1234567890",
  "NombreCompleto": "Aprendiz Ejemplo",
  "Programa": "Análisis y Desarrollo de Software",
  "Ficha": "1234567",
  "FechaMatricula": "2025-02-15T10:30:00",
  "Estado": "Activa"
}
```

El archivo `Data/matriculas.json` almacena un arreglo de objetos con esta estructura, mientras que cada matrícula también se guarda individualmente en `Data/Matriculas/matricula_{id}.json` y en `matricula_{id}.pdf`.

---

## 9. Posibles mejoras

- Integrar autenticación para restringir el acceso.  
- Migrar la persistencia a una base de datos relacional (SQL Server o PostgreSQL).  
- Agregar paginación y filtros avanzados en la vista `Index`.  
- Exponer una API para consumo externo.  
- Incorporar pruebas unitarias para servicios y controlador.

---

**© 2025 · Sistema de Matrícula SENA · Desarrollado con ASP.NET Core**
