using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.Interfaces.Persistence
{
    public interface IPrestamoRepository : IBaseRepository<Prestamo, Guid>
    {
        Task<IEnumerable<Prestamo>> GetByUsuarioIdAsync(Guid usuarioId);
        Task<IEnumerable<Prestamo>> GetActivosByUsuarioIdAsync(Guid usuarioId);
        Task<IEnumerable<Prestamo>> GetAtrasadosAsync();
        Task<IEnumerable<Prestamo>> GetActivosByRecursoIdAsync(Guid recursoId);
    }
}
