using Microsoft.Extensions.DependencyInjection;
using SIGEBI.Application.Interfaces;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Business.Interfaces.Services;
using SIGEBI.Business.Services;
using SIGEBI.Business.UseCases.Catalogo;
using SIGEBI.Business.UseCases.Prestamos;
using SIGEBI.Business.UseCases.Usuarios;
using SIGEBI.Business.Validators;
using SIGEBI.Infrastructure.Persistance;
using SIGEBI.Infrastructure.Persistance.Repositories;
using SIGEBI.Infrastructure.Services;

namespace SIGEBI.Infrastructure.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // Aca le decimos al sistema "cada vez que una clase pida un IUsuarioRepository, pasale una instancia de UsuarioRepository".
            // Así los Casos de Uso dependen de las abstracciones y no de Entity Framework directo.
            
            // Usamos Scoped para casi todo asi vive por cada peticion HTTP (lo vimos en la clase 4)
            
            // Repositorios
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IPrestamoRepository, PrestamoRepository>();
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<IRecursoRepository, RecursoRepository>();
            services.AddScoped<IValoracionRepository, ValoracionRepository>();
            services.AddScoped<IPenalizacionRepository, PenalizacionRepository>();
            services.AddScoped<INotificacionRepository, NotificacionRepository>();
            services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();
            services.AddScoped<IListaDeseosRepository, ListaDeseosRepository>();

            // Unidad de trabajo
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Servicios de infraestructura
            services.AddScoped<IHashService, HashService>();
            services.AddScoped<IEmailAdapter, EmailAdapter>();
            services.AddScoped<IJwtService, JwtService>();

            // Casos de uso - Usuarios
            services.AddScoped<LoginUsuarioUseCase>();
            services.AddScoped<RegistrarUsuarioUseCase>();
            services.AddScoped<GestionarUsuarioUseCase>();
            services.AddScoped<PenalizacionesUseCase>();
            services.AddScoped<NotificacionesUseCase>();
            services.AddScoped<ConsultarAuditoriaUseCase>();
            services.AddScoped<GenerarReportesUseCase>();

            // Casos de uso - Catalogo
            services.AddScoped<ConsultarLibrosUseCase>();
            services.AddScoped<GestionarRecursosUseCase>();
            services.AddScoped<ValoracionesUseCase>();
            services.AddScoped<ListaDeseosUseCase>();
            services.AddScoped<CategoriasUseCase>();

            // Casos de uso - Prestamos
            services.AddScoped<SolicitarPrestamoUseCase>();
            services.AddScoped<DevolverPrestamoUseCase>();
            services.AddScoped<ConsultarPrestamoUseCase>();
            services.AddScoped<EliminarPrestamoUseCase>();

            // Servicios de aplicación
            services.AddScoped<RegistrarPrestamoService>();
            services.AddScoped<GenerarReportesService>();

            // Validadores
            services.AddScoped<RegistrarUsuarioValidator>();
            services.AddScoped<AgregarRecursoValidator>();
            services.AddScoped<SolicitarPrestamoValidator>();
            services.AddScoped<ValoracionValidator>();
            services.AddScoped<ListaDeseosValidator>();

            return services;
        }
    }
}

