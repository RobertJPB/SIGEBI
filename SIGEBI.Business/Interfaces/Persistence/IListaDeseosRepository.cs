using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.Interfaces.Persistence
{
    public interface IListaDeseosRepository : IBaseRepository<ListaDeseos, Guid>
    {
        Task<ListaDeseos?> GetByUsuarioIdAsync(Guid usuarioId);
        Task<IEnumerable<Guid>> GetUsuariosInteresadosAsync(Guid recursoId);
    }
}
