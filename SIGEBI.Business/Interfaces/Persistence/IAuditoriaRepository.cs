using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.Interfaces.Persistence
{
    public interface IAuditoriaRepository
    {
        Task<IEnumerable<Auditoria>> GetAllAsync();
        Task<Auditoria?> GetByIdAsync(int id);
        Task<IEnumerable<Auditoria>> GetByUsuarioIdAsync(Guid usuarioId);
        Task AddAsync(Auditoria entity);
        Task<bool> ExistsAsync(int id);
    }
}