using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.Interfaces.Persistence
{
    public interface INotificacionRepository
    {
        Task<IEnumerable<Notificacion>> GetAllAsync();
        Task<Notificacion?> GetByIdAsync(Guid id);
        Task<IEnumerable<Notificacion>> GetByUsuarioIdAsync(Guid usuarioId);
        Task AddAsync(Notificacion entity);
        void Update(Notificacion entity);
        void Delete(Notificacion entity);
        Task<bool> ExistsAsync(Guid id);
    }
}