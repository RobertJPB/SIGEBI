using SIGEBI.Business.DTOs;

namespace SIGEBI.Business.Interfaces.UseCases.Usuarios
{
    /// <summary>
    /// Contrato para la gestión de notificaciones persistentes de los usuarios.
    /// </summary>
    public interface INotificacionesUseCase
    {
        Task<IEnumerable<NotificacionDTO>> ObtenerPorUsuarioAsync(Guid usuarioId);
        Task<int> ObtenerCantPendientesAsync(Guid usuarioId);
        Task MarcarComoLeidaAsync(Guid notificacionId);
        Task EliminarNotificacionAsync(Guid id);
        Task<NotificacionDTO> ObtenerPorIdAsync(Guid id);
        Task EnviarNotificacionPrestamoAsync(Guid usuarioId, DateTime fechaDevolucion);
    }
}
