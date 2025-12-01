using ProyectoSena2025.Services;

/// <summary>
/// Punto de entrada de la aplicación ASP.NET Core.
/// Aquí se configuran los servicios y el pipeline HTTP de la aplicación.
/// </summary>
var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Registro de servicios en el contenedor de dependencias.
/// </summary>

// Habilita el patrón MVC (Controladores + Vistas Razor).
builder.Services.AddControllersWithViews();

// Registra el servicio de matrículas. Por cada petición HTTP se crea una instancia de MatriculaService.
builder.Services.AddScoped<IMatriculaService, MatriculaService>();

// Construye la aplicación con la configuración anterior.
var app = builder.Build();

/// <summary>
/// Configuración del pipeline de middleware de la aplicación.
/// El orden de los middlewares es importante.
/// </summary>
if (!app.Environment.IsDevelopment())
{
    // En producción, si ocurre una excepción se redirige a la vista /Home/Error.
    app.UseExceptionHandler("/Home/Error");

    // HSTS indica a los navegadores que usen siempre HTTPS.
    app.UseHsts();
}

// Sirve archivos estáticos desde la carpeta wwwroot (css, js, imágenes, etc.).
// IMPORTANTE: debe ejecutarse antes de UseRouting.
app.UseStaticFiles();

// Intenta redirigir las peticiones HTTP a HTTPS cuando sea posible.
app.UseHttpsRedirection();

// Habilita el sistema de enrutamiento de ASP.NET Core.
app.UseRouting();

// Middleware de autorización (aunque este proyecto no implementa autenticación, queda preparado).
app.UseAuthorization();

// Define la ruta por defecto:
//   /           -> Home/Index
//   /Home       -> Home/Index
//   /Home/Crear -> Home/Crear
//   /Home/Editar/5 -> Home/Editar(id:5)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Inicia la aplicación web.
app.Run();

