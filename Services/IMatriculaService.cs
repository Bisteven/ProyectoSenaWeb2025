using ProyectoSena2025.Models;

namespace ProyectoSena2025.Services;

public interface IMatriculaService
{
    Task<List<Matricula>> ObtenerMatriculasAsync();
    Task<Matricula?> ObtenerMatriculaPorIdAsync(int id);
    Task<bool> GuardarMatriculaAsync(Matricula matricula);
    Task<bool> EliminarMatriculaAsync(int id);
}

