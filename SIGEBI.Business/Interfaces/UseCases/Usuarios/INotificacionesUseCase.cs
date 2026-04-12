using SIGEBI.Business.DTOs;

namespace SIGEBI.Business.Interfaces.UseCases.Usuarios
{
    public interface INotificacionesUseCase
    {
        Task<IEnumerable<NotificacionDTO>> ObtenerPorUsuarioAsync(Guid usuarioId);
        Task<int> ObtenerCantPendientesAsync(Guid usuarioId);
        Task MarcarComoLeidaAsync(Guid notificacionId);
        Task EliminarNotificacionAsync(Guid id);
        Task<NotificacionDTO> ObtenerPorIdAsync(Guid id);
        Task MarcarTodasComoLeidasAsync(Guid usuarioId);
        Task<IEnumerable<NotificacionDTO>> ObtenerTodasAsync();
        Task EnviarNotificacionPrestamoAsync(Guid usuarioId, DateTime fechaDevolucion);
        Task EnviarNotificacionAsync(Guid usuarioId, string mensaje);
    }
}

