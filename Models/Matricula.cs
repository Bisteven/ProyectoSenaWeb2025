using System.ComponentModel.DataAnnotations;

namespace ProyectoSena2025.Models;

public class Matricula
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "El número de documento es obligatorio")]
    [Display(Name = "Número de Documento")]
    public string? NumeroDocumento { get; set; }
    
    [Required(ErrorMessage = "El nombre completo es obligatorio")]
    [Display(Name = "Nombre Completo")]
    public string? NombreCompleto { get; set; }
    
    [Required(ErrorMessage = "El programa es obligatorio")]
    [Display(Name = "Programa")]
    public string? Programa { get; set; }
    
    [Required(ErrorMessage = "La ficha es obligatoria")]
    [Display(Name = "Ficha")]
    public string? Ficha { get; set; }
    
    [Display(Name = "Fecha de Matrícula")]
    [DataType(DataType.Date)]
    public DateTime FechaMatricula { get; set; }
    
    [Display(Name = "Estado")]
    public string? Estado { get; set; }
}

