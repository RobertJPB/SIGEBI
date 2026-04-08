using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SIGEBI.Services;
using SIGEBI.ViewModels;
using Refit;
using System.Text.Json;

namespace SIGEBI;

/// <summary>
/// Punto de entrada de SIGEBI.Desktop.
/// Configura el contenedor de inversión de control (IoC) con:
/// - ISigebiApi vía Refit (cliente REST tipado + AuthHandler JWT)
/// - ResourceUploadService (operaciones multipart/form-data)
/// - Todos los ViewModels bajo patrón IsBusy / ManejarErrorAsync
/// </summary>
public partial class App : Application
{
    public new static App Current => (App)Application.Current;
    public IServiceProvider Services { get; }

    public App()
    {
        Services = ConfigureServices();
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // ── Middleware ──
        services.AddTransient<AuthHandler>();

        // ── ISigebiApi (Refit) — Todos los endpoints REST tipados ──
        services.AddRefitClient<ISigebiApi>(new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })
        })
        .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:7047/"))
        .AddHttpMessageHandler<AuthHandler>();

        // ── ResourceUploadService — Operaciones multipart (imágenes de recursos) ──
        services.AddHttpClient<ResourceUploadService>(client =>
        {
            client.BaseAddress = new Uri("https://localhost:7047/");
        })
        .AddHttpMessageHandler<AuthHandler>();

        // ── ViewModels ──
        services.AddTransient<MainViewModel>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<GestionBibliograficaViewModel>();
        services.AddTransient<GestionPrestamosViewModel>();
        services.AddTransient<GestionUsuariosViewModel>();
        services.AddTransient<PenalizacionesViewModel>();
        services.AddTransient<NotificacionesViewModel>();
        services.AddTransient<AuditoriaViewModel>();

        return services.BuildServiceProvider();
    }
}

