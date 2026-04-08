using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.Interfaces.Persistence
{
    public interface INotificacionRepository : IBaseRepository<Notificacion, Guid>
    {
        Task<IEnumerable<Notificacion>> GetByUsuarioIdAsync(Guid usuarioId);
        Task<int> GetCantPendientesAsync(Guid usuarioId);
    }
}
