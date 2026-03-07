using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.Interfaces.Persistance
{
    public interface IPrestamoRepository
    {
        Task<IEnumerable<Prestamo>> GetAllAsync();
        Task<Prestamo?> GetByIdAsync(Guid id);
        Task<IEnumerable<Prestamo>> GetByUsuarioIdAsync(Guid usuarioId);
        Task<IEnumerable<Prestamo>> GetActivosByUsuarioIdAsync(Guid usuarioId);
        Task<IEnumerable<Prestamo>> GetAtrasadosAsync();
        Task AddAsync(Prestamo entity);
        void Update(Prestamo entity);
        void Delete(Prestamo entity);
        Task<bool> ExistsAsync(Guid id);
    }
}