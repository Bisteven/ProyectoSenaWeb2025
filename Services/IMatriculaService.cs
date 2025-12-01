using ProyectoSena2025.Models;

namespace ProyectoSena2025.Services;

/// <summary>
/// Contrato que define las operaciones necesarias para gestionar matrículas.
/// Implementado por <see cref="MatriculaService" />.
/// </summary>
public interface IMatriculaService
{
    /// <summary>
    /// Obtiene todas las matrículas almacenadas en el sistema.
    /// </summary>
    /// <returns>Lista de matrículas, o una lista vacía si no hay registros.</returns>
    Task<List<Matricula>> ObtenerMatriculasAsync();

    /// <summary>
    /// Busca una matrícula específica por su identificador.
    /// </summary>
    /// <param name="id">Identificador de la matrícula.</param>
    /// <returns>La matrícula encontrada, o null si no existe.</returns>
    Task<Matricula?> ObtenerMatriculaPorIdAsync(int id);

    /// <summary>
    /// Guarda una matrícula.
    /// Si la matrícula tiene Id 0 se crea un nuevo registro, de lo contrario se actualiza el existente.
    /// También genera el archivo JSON y el PDF individuales.
    /// </summary>
    /// <param name="matricula">Datos de la matrícula a guardar.</param>
    /// <returns>true si la operación fue exitosa, false en caso contrario.</returns>
    Task<bool> GuardarMatriculaAsync(Matricula matricula);

    /// <summary>
    /// Elimina una matrícula por su identificador,
    /// actualizando el archivo principal y eliminando los archivos JSON y PDF correspondientes.
    /// </summary>
    /// <param name="id">Identificador de la matrícula a eliminar.</param>
    /// <returns>true si se eliminó correctamente, false si no se encontró.</returns>
    Task<bool> EliminarMatriculaAsync(int id);
}

