using Microsoft.Extensions.DependencyInjection;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.Interfaces.Services;
using SIGEBI.Infrastructure.Persistence;
using SIGEBI.Infrastructure.Persistence.Repositories;
using SIGEBI.Infrastructure.Persistence.Interceptors;
using SIGEBI.Infrastructure.Services;
using SIGEBI.Infrastructure.Common;
using SIGEBI.Business.Interfaces.Common;

namespace SIGEBI.Infrastructure.IoC
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddHttpContextAccessor(); // requerido para interceptores y auditoría
            // Repositorios: cada vez que una clase pida IXRepository, se provee la implementación concreta.
            // Scoped → una instancia por petición HTTP, compartida dentro de la misma request.
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IPrestamoRepository, PrestamoRepository>();
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<IRecursoRepository, RecursoRepository>();
            services.AddScoped<IValoracionRepository, ValoracionRepository>();
            services.AddScoped<IPenalizacionRepository, PenalizacionRepository>();
            services.AddScoped<INotificacionRepository, NotificacionRepository>();
            services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();
            services.AddScoped<IListaDeseosRepository, ListaDeseosRepository>();
            services.AddScoped<IReporteRepository, ReporteRepository>();

            // Unidad de trabajo: coordina todos los repositorios en una sola transacción.
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Servicios técnicos de infraestructura (hashing, correo, tokens JWT).
            services.AddScoped<IHashService, HashService>();
            services.AddSingleton<IEmailAdapter, EmailAdapter>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddSingleton<AuditoriaInterceptor>();
            services.AddSingleton<IGuidGenerator, SequentialGuidGenerator>(); // Registro del generador
            services.AddScoped<IAuditService, AuditService>();
            services.AddScoped<IImagenService, ImagenService>();
            services.AddScoped<IDomainEventDispatcher, ManualDomainEventDispatcher>();

            return services;
        }
    }
}

