using Microsoft.AspNetCore.Mvc;
using ProyectoSena2025.Models;
using ProyectoSena2025.Services;

namespace ProyectoSena2025.Controllers;

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
            // Si el archivo no existe, generarlo
            await _matriculaService.GuardarMatriculaAsync(matricula);
            filePath = Path.Combine(_environment.ContentRootPath, "Data", "Matriculas", $"matricula_{id}.json");
        }

        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
        var fileName = $"matricula_{id}.json";
        
        return File(fileBytes, "application/json", fileName);
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
            // Si el archivo no existe, generarlo
            await _matriculaService.GuardarMatriculaAsync(matricula);
            filePath = Path.Combine(_environment.ContentRootPath, "Data", "Matriculas", $"matricula_{id}.pdf");
        }

        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
        var fileName = $"matricula_{id}.pdf";
        
        return File(fileBytes, "application/pdf", fileName);
    }
}

