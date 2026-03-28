using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.Interfaces.Persistence
{
    public interface IPenalizacionRepository : IBaseRepository<Penalizacion, Guid>
    {
        Task<IEnumerable<Penalizacion>> GetByUsuarioIdAsync(Guid usuarioId);
        Task<IEnumerable<Penalizacion>> GetActivasAsync();
        Task<Penalizacion?> GetByPrestamoIdAsync(Guid prestamoId);
    }
}
