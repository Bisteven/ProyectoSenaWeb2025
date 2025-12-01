using System.ComponentModel.DataAnnotations;

namespace ProyectoSena2025.Models;

/// <summary>
/// Representa la información básica de una matrícula de aprendiz en el sistema.
/// Este modelo se usa tanto en las vistas como en la persistencia en archivos JSON/PDF.
/// </summary>
public class Matricula
{
    /// <summary>
    /// Identificador único de la matrícula.
    /// Se asigna automáticamente en el servicio al crear una nueva matrícula.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Número de documento del aprendiz (CC, TI, etc.).
    /// Campo obligatorio.
    /// </summary>
    [Required(ErrorMessage = "El número de documento es obligatorio")]
    [Display(Name = "Número de Documento")]
    public string? NumeroDocumento { get; set; }
    
    /// <summary>
    /// Nombre completo del aprendiz.
    /// Campo obligatorio.
    /// </summary>
    [Required(ErrorMessage = "El nombre completo es obligatorio")]
    [Display(Name = "Nombre Completo")]
    public string? NombreCompleto { get; set; }
    
    /// <summary>
    /// Programa de formación del SENA (por ejemplo: Análisis y Desarrollo de Software).
    /// Campo obligatorio.
    /// </summary>
    [Required(ErrorMessage = "El programa es obligatorio")]
    [Display(Name = "Programa")]
    public string? Programa { get; set; }
    
    /// <summary>
    /// Número de ficha asociado al programa de formación.
    /// Campo obligatorio.
    /// </summary>
    [Required(ErrorMessage = "La ficha es obligatoria")]
    [Display(Name = "Ficha")]
    public string? Ficha { get; set; }
    
    /// <summary>
    /// Fecha en que se registró la matrícula.
    /// Normalmente se establece a DateTime.Now al crear una nueva matrícula.
    /// </summary>
    [Display(Name = "Fecha de Matrícula")]
    [DataType(DataType.Date)]
    public DateTime FechaMatricula { get; set; }
    
    /// <summary>
    /// Estado actual de la matrícula (Activa, Inactiva, Cancelada, etc.).
    /// </summary>
    [Display(Name = "Estado")]
    public string? Estado { get; set; }
}

