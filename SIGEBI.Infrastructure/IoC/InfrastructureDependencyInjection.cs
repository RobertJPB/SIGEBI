using Microsoft.Extensions.DependencyInjection;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.Interfaces.Services;
using SIGEBI.Infrastructure.Persistence;
using SIGEBI.Infrastructure.Persistence.Repositories;
using SIGEBI.Infrastructure.Persistence.Interceptors;
using SIGEBI.Infrastructure.Services;

namespace SIGEBI.Infrastructure.IoC
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
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

            // Unidad de trabajo: coordina todos los repositorios en una sola transacción.
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Servicios técnicos de infraestructura (hashing, correo, tokens JWT).
            services.AddScoped<IHashService, HashService>();
            services.AddScoped<IEmailAdapter, EmailAdapter>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<AuditoriaInterceptor>();

            return services;
        }
    }
}

