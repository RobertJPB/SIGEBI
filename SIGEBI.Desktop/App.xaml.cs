using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SIGEBI.Services;
using SIGEBII.ViewModels;

namespace SIGEBII;

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

        // ── ViewModels ──
        services.AddTransient<MainViewModel>();
        services.AddTransient<SIGEBI.ViewModels.LoginViewModel>();
        services.AddTransient<SIGEBI.ViewModels.GestionBibliograficaViewModel>();

        // ── Services ──
        services.AddTransient<AuthHandler>();
        
        services.AddHttpClient<ApiService>(client =>
        {
            client.BaseAddress = new Uri("https://localhost:7047/");
        })
        .AddHttpMessageHandler<AuthHandler>();

        return services.BuildServiceProvider();
    }
}
