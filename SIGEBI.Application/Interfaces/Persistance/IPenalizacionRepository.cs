using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.Interfaces.Persistance
{
    public interface IPenalizacionRepository
    {
        Task<IEnumerable<Penalizacion>> GetAllAsync();
        Task<Penalizacion?> GetByIdAsync(Guid id);
        Task<IEnumerable<Penalizacion>> GetByUsuarioIdAsync(Guid usuarioId);
        Task<IEnumerable<Penalizacion>> GetActivasAsync();
        Task AddAsync(Penalizacion entity);
        void Update(Penalizacion entity);
        void Delete(Penalizacion entity);
        Task<bool> ExistsAsync(Guid id);
    }
}