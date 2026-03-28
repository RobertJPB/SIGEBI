using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.Interfaces.Persistence
{
    public interface IAuditoriaRepository : IBaseRepository<Auditoria, int>
    {
        Task<IEnumerable<Auditoria>> GetByUsuarioIdAsync(Guid usuarioId);
    }
}