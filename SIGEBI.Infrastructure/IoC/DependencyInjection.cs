using Microsoft.Extensions.DependencyInjection;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Business.Interfaces.Services;
using SIGEBI.Infrastructure.Persistance.Repositories;
using SIGEBI.Infrastructure.Services;

namespace SIGEBI.Infrastructure.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // Repositories
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IPrestamoRepository, PrestamoRepository>();
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<IRecursoRepository, RecursoRepository>();
            services.AddScoped<IValoracionRepository, ValoracionRepository>();

            // Services
            services.AddScoped<IHashService, HashService>();
            services.AddScoped<IEmailService, EmailService>();

            return services;
        }
    }
}