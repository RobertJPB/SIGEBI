using Microsoft.Extensions.DependencyInjection;
using SIGEBI.Business.Services;
using SIGEBI.Business.UseCases.Catalogo;
using SIGEBI.Business.UseCases.Prestamos;
using SIGEBI.Business.UseCases.Usuarios;
using SIGEBI.Business.Validators;

namespace SIGEBI.Business.IoC
{
    public static class BusinessDependencyInjection
    {
        public static IServiceCollection AddBusiness(this IServiceCollection services)
        {
            // ── Casos de Uso: Usuarios ──
            services.AddScoped<LoginUsuarioUseCase>();
            services.AddScoped<RegistrarUsuarioUseCase>();
            services.AddScoped<GestionarUsuarioUseCase>();
            services.AddScoped<PenalizacionesUseCase>();
            services.AddScoped<NotificacionesUseCase>();
            services.AddScoped<ConsultarAuditoriaUseCase>();
            services.AddScoped<GenerarReportesUseCase>();

            // ── Casos de Uso: Catálogo ──
            services.AddScoped<ConsultarLibrosUseCase>();
            services.AddScoped<GestionarRecursosUseCase>();
            services.AddScoped<ValoracionesUseCase>();
            services.AddScoped<ListaDeseosUseCase>();
            services.AddScoped<CategoriasUseCase>();

            // ── Casos de Uso: Préstamos ──
            services.AddScoped<SolicitarPrestamoUseCase>();
            services.AddScoped<DevolverPrestamoUseCase>();
            services.AddScoped<ConsultarPrestamoUseCase>();
            services.AddScoped<EliminarPrestamoUseCase>();

            // ── Servicios de Aplicación ──
            services.AddScoped<RegistrarPrestamoService>();
            services.AddScoped<GenerarReportesService>();

            // ── Validadores ──
            services.AddScoped<RegistrarUsuarioValidator>();
            services.AddScoped<AgregarRecursoValidator>();
            services.AddScoped<SolicitarPrestamoValidator>();
            services.AddScoped<ValoracionValidator>();
            services.AddScoped<ListaDeseosValidator>();

            return services;
        }
    }
}
