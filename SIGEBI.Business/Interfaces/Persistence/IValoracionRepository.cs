using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.Interfaces.Persistence
{
    public interface IValoracionRepository : IBaseRepository<Valoracion, Guid>
    {
        Task<IEnumerable<Valoracion>> GetByRecursoIdAsync(Guid recursoId);
        Task<IEnumerable<Valoracion>> GetByUsuarioIdAsync(Guid usuarioId);
        Task<double> GetPromedioCalificacionAsync(Guid recursoId);
        Task<Dictionary<Guid, double>> GetPromediosBatchAsync(IEnumerable<Guid> recursoIds);
    }
}
