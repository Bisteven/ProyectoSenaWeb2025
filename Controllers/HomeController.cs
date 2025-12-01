using Microsoft.AspNetCore.Mvc;
using ProyectoSena2025.Models;
using ProyectoSena2025.Services;

namespace ProyectoSena2025.Controllers;

/// <summary>
/// Controlador principal del sistema de matrículas.
/// Gestiona las operaciones de:
/// - Listar matrículas
/// - Crear nuevas matrículas
/// - Editar matrículas
/// - Eliminar matrículas
/// - Descargar archivos JSON y PDF
/// </summary>
public class HomeController : Controller
{
    private readonly IMatriculaService _matriculaService;
    private readonly IWebHostEnvironment _environment;

    /// <summary>
    /// Constructor con inyección de dependencias.
    /// </summary>
    /// <param name="matriculaService">Servicio que gestiona la lógica de negocio de matrículas.</param>
    /// <param name="environment">Información del entorno de ejecución (rutas físicas, etc.).</param>
    public HomeController(IMatriculaService matriculaService, IWebHostEnvironment environment)
    {
        _matriculaService = matriculaService;
        _environment = environment;
    }

    /// <summary>
    /// Muestra la lista de todas las matrículas almacenadas.
    /// GET: /Home/Index
    /// </summary>
    public async Task<IActionResult> Index()
    {
        var matriculas = await _matriculaService.ObtenerMatriculasAsync();
        return View(matriculas);
    }

    /// <summary>
    /// Muestra el formulario para crear una nueva matrícula.
    /// GET: /Home/Crear
    /// </summary>
    [HttpGet]
    public IActionResult Crear()
    {
        return View(new Matricula());
    }

    /// <summary>
    /// Procesa el envío del formulario de creación de matrícula.
    /// Si el modelo es válido, se guarda y se redirige al listado.
    /// POST: /Home/Crear
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(Matricula matricula)
    {
        if (ModelState.IsValid)
        {
            await _matriculaService.GuardarMatriculaAsync(matricula);
            return RedirectToAction(nameof(Index));
        }

        // Si hay errores de validación se vuelve a mostrar el formulario con los mensajes.
        return View(matricula);
    }

    /// <summary>
    /// Muestra el formulario para editar una matrícula existente.
    /// GET: /Home/Editar/{id}
    /// </summary>
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

    /// <summary>
    /// Procesa el formulario de edición de matrícula.
    /// POST: /Home/Editar
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(Matricula matricula)
    {
        if (ModelState.IsValid)
        {
            await _matriculaService.GuardarMatriculaAsync(matricula);
            return RedirectToAction(nameof(Index));
        }

        // Si hay errores de validación se vuelve a mostrar el formulario.
        return View(matricula);
    }

    /// <summary>
    /// Elimina una matrícula por su identificador.
    /// POST: /Home/Eliminar
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _matriculaService.EliminarMatriculaAsync(id);
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Descarga el archivo JSON individual de una matrícula.
    /// Si no existe, se vuelve a guardar la matrícula para generarlo.
    /// GET: /Home/DescargarJson/{id}
    /// </summary>
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
            // Si el archivo no existe, se genera persistiendo nuevamente la matrícula.
            await _matriculaService.GuardarMatriculaAsync(matricula);
            filePath = Path.Combine(_environment.ContentRootPath, "Data", "Matriculas", $"matricula_{id}.json");
        }

        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
        var fileName = $"matricula_{id}.json";
        
        return File(fileBytes, "application/json", fileName);
    }

    /// <summary>
    /// Descarga el comprobante de matrícula en formato PDF.
    /// Si el archivo no existe, se genera antes de devolverlo.
    /// GET: /Home/DescargarPdf/{id}
    /// </summary>
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
            // Si el archivo no existe, se genera persistiendo nuevamente la matrícula.
            await _matriculaService.GuardarMatriculaAsync(matricula);
            filePath = Path.Combine(_environment.ContentRootPath, "Data", "Matriculas", $"matricula_{id}.pdf");
        }

        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
        var fileName = $"matricula_{id}.pdf";
        
        return File(fileBytes, "application/pdf", fileName);
    }
}

