using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SIGEBI.Services;
using SIGEBI.ViewModels;
using Refit;
using System.Text.Json;

namespace SIGEBI;
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

        var refitSettings = new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })
        };

        void ConfigureClient<T>(IServiceCollection services) where T : class
        {
            services.AddRefitClient<T>(refitSettings)
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:7047/"))
                .AddHttpMessageHandler<AuthHandler>();
        }

        ConfigureClient<IAuthApi>(services);
        ConfigureClient<IUsuariosApi>(services);
        ConfigureClient<IRecursosApi>(services);
        ConfigureClient<ICategoriasApi>(services);
        ConfigureClient<IPrestamosApi>(services);
        ConfigureClient<IPenalizacionesApi>(services);
        ConfigureClient<INotificacionesApi>(services);
        ConfigureClient<IAuditoriaApi>(services);
        ConfigureClient<IValoracionesApi>(services);
        ConfigureClient<IListaDeseosApi>(services);
        ConfigureClient<IReportesApi>(services);

        // ── Facade ──
        services.AddTransient<ISigebiApiFacade, SigebiApiFacade>();

        // ── ResourceUploadService — Operaciones multipart (imágenes de recursos) ──
        services.AddHttpClient<ResourceUploadService>(client =>
        {
            client.BaseAddress = new Uri("https://localhost:7047/");
        })
        .AddHttpMessageHandler<AuthHandler>();

        // ── ViewModels ──
        services.AddSingleton<MainViewModel>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<GestionBibliograficaViewModel>();
        services.AddTransient<GestionPrestamosViewModel>();
        services.AddTransient<GestionUsuariosViewModel>();
        services.AddTransient<PenalizacionesViewModel>();
        services.AddTransient<NotificacionesViewModel>();
        services.AddTransient<AuditoriaViewModel>();
        services.AddTransient<ReportesViewModel>();

        return services.BuildServiceProvider();
    }
}


