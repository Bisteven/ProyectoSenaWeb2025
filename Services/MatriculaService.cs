using System.Text.Json;
using ProyectoSena2025.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ProyectoSena2025.Services;

public class MatriculaService : IMatriculaService
{
    private readonly string _jsonFilePath;
    private readonly IWebHostEnvironment _environment;

    public MatriculaService(IWebHostEnvironment environment)
    {
        _environment = environment;
        _jsonFilePath = Path.Combine(_environment.ContentRootPath, "Data", "matriculas.json");
        
        // Asegurar que el directorio existe
        var directory = Path.GetDirectoryName(_jsonFilePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory!);
        }
    }

    public async Task<List<Matricula>> ObtenerMatriculasAsync()
    {
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

    public async Task<Matricula?> ObtenerMatriculaPorIdAsync(int id)
    {
        var matriculas = await ObtenerMatriculasAsync();
        return matriculas.FirstOrDefault(m => m.Id == id);
    }

    public async Task<bool> GuardarMatriculaAsync(Matricula matricula)
    {
        var matriculas = await ObtenerMatriculasAsync();
        
        if (matricula.Id == 0)
        {
            // Nueva matrícula
            matricula.Id = matriculas.Count > 0 ? matriculas.Max(m => m.Id) + 1 : 1;
            matricula.FechaMatricula = DateTime.Now;
            matricula.Estado = matricula.Estado ?? "Activa";
            matriculas.Add(matricula);
        }
        else
        {
            // Actualizar matrícula existente
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

        // Guardar todas las matrículas en el archivo principal
        var json = JsonSerializer.Serialize(matriculas, options);
        await File.WriteAllTextAsync(_jsonFilePath, json);
        
        // Crear archivo JSON individual para la matrícula
        var matriculaDirectory = Path.Combine(_environment.ContentRootPath, "Data", "Matriculas");
        if (!Directory.Exists(matriculaDirectory))
        {
            Directory.CreateDirectory(matriculaDirectory);
        }
        
        var matriculaJsonPath = Path.Combine(matriculaDirectory, $"matricula_{matricula.Id}.json");
        var matriculaJson = JsonSerializer.Serialize(matricula, options);
        await File.WriteAllTextAsync(matriculaJsonPath, matriculaJson);
        
        // Crear archivo PDF con la información de la matrícula
        var matriculaPdfPath = Path.Combine(matriculaDirectory, $"matricula_{matricula.Id}.pdf");
        await GenerarPdfMatriculaAsync(matricula, matriculaPdfPath);
        
        return true;
    }

    public async Task<bool> EliminarMatriculaAsync(int id)
    {
        var matriculas = await ObtenerMatriculasAsync();
        var matricula = matriculas.FirstOrDefault(m => m.Id == id);
        
        if (matricula == null)
        {
            return false;
        }

        matriculas.Remove(matricula);

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        // Actualizar archivo principal
        var json = JsonSerializer.Serialize(matriculas, options);
        await File.WriteAllTextAsync(_jsonFilePath, json);
        
        // Eliminar archivo JSON individual de la matrícula
        var matriculaJsonPath = Path.Combine(_environment.ContentRootPath, "Data", "Matriculas", $"matricula_{id}.json");
        if (File.Exists(matriculaJsonPath))
        {
            File.Delete(matriculaJsonPath);
        }
        
        // Eliminar archivo PDF de la matrícula
        var matriculaPdfPath = Path.Combine(_environment.ContentRootPath, "Data", "Matriculas", $"matricula_{id}.pdf");
        if (File.Exists(matriculaPdfPath))
        {
            File.Delete(matriculaPdfPath);
        }
        
        return true;
    }
    
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

                page.Header()
                    .AlignCenter()
                    .Text("SISTEMA DE MATRÍCULA - SENA 2025")
                    .FontSize(20)
                    .Bold()
                    .FontColor(Colors.Blue.Darken3);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(column =>
                    {
                        column.Spacing(20);

                        // Título
                        column.Item()
                            .AlignCenter()
                            .Text("COMPROBANTE DE MATRÍCULA")
                            .FontSize(18)
                            .Bold()
                            .FontColor(Colors.Blue.Darken2);

                        // Información de la matrícula
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

                        // Pie de página
                        column.Item()
                            .AlignCenter()
                            .PaddingTop(20)
                            .Text($"Archivo generado el: {DateTime.Now:dd/MM/yyyy HH:mm:ss}")
                            .FontSize(10)
                            .FontColor(Colors.Grey.Darken1);
                    });

                page.Footer()
                    .AlignCenter()
                    .Text("Servicio Nacional de Aprendizaje - SENA")
                    .FontSize(10)
                    .FontColor(Colors.Grey.Darken2);
            });
        });

        await Task.Run(() => document.GeneratePdf(filePath));
    }

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

