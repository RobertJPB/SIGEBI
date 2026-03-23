using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.Interfaces.Persistance
{
    public interface IValoracionRepository
    {
        Task<IEnumerable<Valoracion>> GetAllAsync();
        Task<Valoracion?> GetByIdAsync(Guid id);
        Task<IEnumerable<Valoracion>> GetByRecursoIdAsync(Guid recursoId);
        Task<IEnumerable<Valoracion>> GetByUsuarioIdAsync(Guid usuarioId);
        Task<double> GetPromedioCalificacionAsync(Guid recursoId);
        Task AddAsync(Valoracion entity);
        void Update(Valoracion entity);
        void Delete(Valoracion entity);
        Task<bool> ExistsAsync(Guid id);
    }
}