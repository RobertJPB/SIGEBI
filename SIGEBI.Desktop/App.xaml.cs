using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SIGEBI.Services;
using SIGEBI.Business.DTOs;
using SIGEBI.ViewModels;

namespace SIGEBI;

/// <summary>
/// Interaction logic for App.xaml
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

        // ── Services ──
        services.AddTransient<AuthHandler>();
        
        services.AddHttpClient<ApiService>(client =>
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
