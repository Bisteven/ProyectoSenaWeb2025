using System.Text.Json;
using ProyectoSena2025.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ProyectoSena2025.Services;

/// <summary>
/// Implementación del servicio de matrículas.
/// Se encarga de:
/// - Leer y escribir el archivo principal Data/matriculas.json
/// - Crear/actualizar archivos individuales JSON y PDF para cada matrícula
/// </summary>
public class MatriculaService : IMatriculaService
{
    private readonly string _jsonFilePath;
    private readonly IWebHostEnvironment _environment;

    /// <summary>
    /// Constructor del servicio.
    /// Inicializa la ruta del archivo JSON principal y asegura que exista el directorio Data/.
    /// </summary>
    public MatriculaService(IWebHostEnvironment environment)
    {
        _environment = environment;
        _jsonFilePath = Path.Combine(_environment.ContentRootPath, "Data", "matriculas.json");
        
        // Asegurar que el directorio Data existe.
        var directory = Path.GetDirectoryName(_jsonFilePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory!);
        }
    }

    /// <summary>
    /// Obtiene todas las matrículas leyendo el archivo principal JSON.
    /// </summary>
    public async Task<List<Matricula>> ObtenerMatriculasAsync()
    {
        // Si el archivo aún no existe, se retorna una lista vacía.
        if (!File.Exists(_jsonFilePath))
        {
            return new List<Matricula>();
        }

        var json = await File.ReadAllTextAsync(_jsonFilePath);
        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<Matricula>();
        }

        var matriculas = JsonSerializer.Deserialize<List<Matricula>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return matriculas ?? new List<Matricula>();
    }

    /// <summary>
    /// Obtiene una matrícula específica por su Id.
    /// </summary>
    public async Task<Matricula?> ObtenerMatriculaPorIdAsync(int id)
    {
        var matriculas = await ObtenerMatriculasAsync();
        return matriculas.FirstOrDefault(m => m.Id == id);
    }

    /// <summary>
    /// Guarda una matrícula nueva o actualiza una existente.
    /// También actualiza el archivo principal y genera los archivos JSON y PDF individuales.
    /// </summary>
    public async Task<bool> GuardarMatriculaAsync(Matricula matricula)
    {
        var matriculas = await ObtenerMatriculasAsync();
        
        if (matricula.Id == 0)
        {
            // Nueva matrícula: se calcula el siguiente Id disponible.
            matricula.Id = matriculas.Count > 0 ? matriculas.Max(m => m.Id) + 1 : 1;
            matricula.FechaMatricula = DateTime.Now;
            matricula.Estado = matricula.Estado ?? "Activa";
            matriculas.Add(matricula);
        }
        else
        {
            // Actualizar matrícula existente.
            var index = matriculas.FindIndex(m => m.Id == matricula.Id);
            if (index >= 0)
            {
                matriculas[index] = matricula;
            }
            else
            {
                // No se encontró la matrícula a actualizar.
                return false;
            }
        }

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        // Guardar todas las matrículas en el archivo principal.
        var json = JsonSerializer.Serialize(matriculas, options);
        await File.WriteAllTextAsync(_jsonFilePath, json);
        
        // Directorio para archivos individuales de cada matrícula.
        var matriculaDirectory = Path.Combine(_environment.ContentRootPath, "Data", "Matriculas");
        if (!Directory.Exists(matriculaDirectory))
        {
            Directory.CreateDirectory(matriculaDirectory);
        }
        
        // Archivo JSON individual.
        var matriculaJsonPath = Path.Combine(matriculaDirectory, $"matricula_{matricula.Id}.json");
        var matriculaJson = JsonSerializer.Serialize(matricula, options);
        await File.WriteAllTextAsync(matriculaJsonPath, matriculaJson);
        
        // Archivo PDF con la información de la matrícula.
        var matriculaPdfPath = Path.Combine(matriculaDirectory, $"matricula_{matricula.Id}.pdf");
        await GenerarPdfMatriculaAsync(matricula, matriculaPdfPath);
        
        return true;
    }

    /// <summary>
    /// Elimina una matrícula y sus archivos asociados (JSON y PDF).
    /// </summary>
    public async Task<bool> EliminarMatriculaAsync(int id)
    {
        var matriculas = await ObtenerMatriculasAsync();
        var matricula = matriculas.FirstOrDefault(m => m.Id == id);
        
        if (matricula == null)
        {
            return false;
        }

        // Quitar de la lista en memoria.
        matriculas.Remove(matricula);

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        // Actualizar archivo principal.
        var json = JsonSerializer.Serialize(matriculas, options);
        await File.WriteAllTextAsync(_jsonFilePath, json);
        
        // Eliminar archivo JSON individual de la matrícula.
        var matriculaJsonPath = Path.Combine(_environment.ContentRootPath, "Data", "Matriculas", $"matricula_{id}.json");
        if (File.Exists(matriculaJsonPath))
        {
            File.Delete(matriculaJsonPath);
        }
        
        // Eliminar archivo PDF de la matrícula.
        var matriculaPdfPath = Path.Combine(_environment.ContentRootPath, "Data", "Matriculas", $"matricula_{id}.pdf");
        if (File.Exists(matriculaPdfPath))
        {
            File.Delete(matriculaPdfPath);
        }
        
        return true;
    }
    
    /// <summary>
    /// Genera un comprobante de matrícula en formato PDF usando QuestPDF.
    /// </summary>
    /// <param name="matricula">Datos de la matrícula a imprimir.</param>
    /// <param name="filePath">Ruta completa donde se guardará el archivo PDF.</param>
    private async Task GenerarPdfMatriculaAsync(Matricula matricula, string filePath)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                // Encabezado principal del documento.
                page.Header()
                    .AlignCenter()
                    .Text("SISTEMA DE MATRÍCULA - SENA 2025")
                    .FontSize(20)
                    .Bold()
                    .FontColor(Colors.Blue.Darken3);

                // Contenido con la información de la matrícula.
                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(column =>
                    {
                        column.Spacing(20);

                        // Título del comprobante.
                        column.Item()
                            .AlignCenter()
                            .Text("COMPROBANTE DE MATRÍCULA")
                            .FontSize(18)
                            .Bold()
                            .FontColor(Colors.Blue.Darken2);

                        // Bloque con los campos de la matrícula.
                        column.Item()
                            .Padding(15)
                            .Background(Colors.Grey.Lighten3)
                            .Border(1)
                            .BorderColor(Colors.Blue.Darken1)
                            .Column(infoColumn =>
                            {
                                infoColumn.Spacing(10);
                                
                                AgregarCampoPdf(infoColumn, "ID de Matrícula", matricula.Id.ToString());
                                AgregarCampoPdf(infoColumn, "Número de Documento", matricula.NumeroDocumento ?? "N/A");
                                AgregarCampoPdf(infoColumn, "Nombre Completo", matricula.NombreCompleto ?? "N/A");
                                AgregarCampoPdf(infoColumn, "Programa", matricula.Programa ?? "N/A");
                                AgregarCampoPdf(infoColumn, "Ficha", matricula.Ficha ?? "N/A");
                                AgregarCampoPdf(infoColumn, "Fecha de Matrícula", matricula.FechaMatricula.ToString("dd/MM/yyyy HH:mm:ss"));
                                AgregarCampoPdf(infoColumn, "Estado", matricula.Estado ?? "N/A");
                            });

                        // Pie de página dentro del contenido con fecha de generación.
                        column.Item()
                            .AlignCenter()
                            .PaddingTop(20)
                            .Text($"Archivo generado el: {DateTime.Now:dd/MM/yyyy HH:mm:ss}")
                            .FontSize(10)
                            .FontColor(Colors.Grey.Darken1);
                    });

                // Pie de página global del documento.
                page.Footer()
                    .AlignCenter()
                    .Text("Servicio Nacional de Aprendizaje - SENA")
                    .FontSize(10)
                    .FontColor(Colors.Grey.Darken2);
            });
        });

        await Task.Run(() => document.GeneratePdf(filePath));
    }
    
    /// <summary>
    /// Agrega una fila etiqueta/valor al documento PDF.
    /// </summary>
    /// <param name="column">Columna de QuestPDF donde se añadirá el campo.</param>
    /// <param name="etiqueta">Texto de la etiqueta (ej: "Programa").</param>
    /// <param name="valor">Valor asociado a la etiqueta.</param>
    private void AgregarCampoPdf(ColumnDescriptor column, string etiqueta, string valor)
    {
        column.Item()
            .Row(row =>
            {
                row.ConstantItem(150)
                    .Text(etiqueta + ":")
                    .Bold()
                    .FontColor(Colors.Blue.Darken3);
                
                row.RelativeItem()
                    .Text(valor)
                    .FontColor(Colors.Black);
            });
    }
}


