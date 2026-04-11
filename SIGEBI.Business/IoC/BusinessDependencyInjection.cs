using Microsoft.Extensions.DependencyInjection;
using SIGEBI.Business.Interfaces.UseCases.Catalogo;
using SIGEBI.Business.Interfaces.UseCases.Prestamos;
using SIGEBI.Business.Interfaces.UseCases.Usuarios;
using SIGEBI.Business.Services;
using SIGEBI.Business.UseCases.Catalogo;
using SIGEBI.Business.UseCases.Prestamos;
using SIGEBI.Business.UseCases.Usuarios;
using SIGEBI.Business.Interfaces.Validators;
using SIGEBI.Business.Validators;
using SIGEBI.Domain.Interfaces.Services;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Business.Interfaces.Services;
using SIGEBI.Business.Services;

namespace SIGEBI.Business.IoC
{
    public static class BusinessDependencyInjection
    {
        public static IServiceCollection AddBusiness(this IServiceCollection services)
        {
            // ── Políticas de Dominio ──
            services.AddScoped<IPrestamoPolicy, PrestamoPolicy>();

            // ── Casos de Uso: Usuarios ──
            services.AddScoped<ILoginUsuarioUseCase, LoginUsuarioUseCase>();
            services.AddScoped<IRegistrarUsuarioUseCase, RegistrarUsuarioUseCase>();
            services.AddScoped<IGestionarUsuarioUseCase, GestionarUsuarioUseCase>();
            services.AddScoped<IPenalizacionesUseCase, PenalizacionesUseCase>();
            services.AddScoped<INotificacionesUseCase, NotificacionesUseCase>();
            services.AddScoped<IConsultarAuditoriaUseCase, ConsultarAuditoriaUseCase>();
            services.AddScoped<IGenerarReportesUseCase, GenerarReportesUseCase>();

            // ── Casos de Uso: Catálogo ──
            services.AddScoped<IConsultarLibrosUseCase, ConsultarLibrosUseCase>();
            services.AddScoped<IGestionarRecursosUseCase, GestionarRecursosUseCase>();
            services.AddScoped<IValoracionesUseCase, ValoracionesUseCase>();
            services.AddScoped<IListaDeseosUseCase, ListaDeseosUseCase>();
            services.AddScoped<ICategoriasUseCase, CategoriasUseCase>();

            // ── Casos de Uso: Préstamos ──
            services.AddScoped<ISolicitarPrestamoUseCase, SolicitarPrestamoUseCase>();
            services.AddScoped<IDevolverPrestamoUseCase, DevolverPrestamoUseCase>();
            services.AddScoped<IConsultarPrestamoUseCase, ConsultarPrestamoUseCase>();
            services.AddScoped<IEliminarPrestamoUseCase, EliminarPrestamoUseCase>();

            // ── Servicios de Aplicación ──
            services.AddScoped<GenerarReportesService>();
            services.AddScoped<IStockNotificationService, StockNotificationService>();

            // ── Validadores ──
            services.AddScoped<IRegistrarUsuarioValidator, RegistrarUsuarioValidator>();
            services.AddScoped<AgregarRecursoValidator>();
            services.AddScoped<SolicitarPrestamoValidator>();
            services.AddScoped<ValoracionValidator>();
            services.AddScoped<ListaDeseosValidator>();

            return services;
        }
    }
}
