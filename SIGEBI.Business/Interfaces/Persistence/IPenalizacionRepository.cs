using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.Interfaces.Persistence
{
    // Define operaciones unicas para multas y sanciones del usuario.
    public interface IPenalizacionRepository
    {
        Task<IEnumerable<Penalizacion>> GetAllAsync();
        Task<Penalizacion?> GetByIdAsync(Guid id);
        Task<IEnumerable<Penalizacion>> GetByUsuarioIdAsync(Guid usuarioId);
        Task<IEnumerable<Penalizacion>> GetActivasAsync();
        Task<Penalizacion?> GetByPrestamoIdAsync(Guid prestamoId);
        Task AddAsync(Penalizacion entity);
        void Update(Penalizacion entity);
        void Delete(Penalizacion entity);
        Task<bool> ExistsAsync(Guid id);
    }
}
