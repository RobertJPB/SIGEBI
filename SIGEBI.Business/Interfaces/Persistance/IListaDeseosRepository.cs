using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.Interfaces.Persistance
{
    public interface IListaDeseosRepository
    {
        Task<ListaDeseos?> GetByUsuarioIdAsync(Guid usuarioId);
        Task<ListaDeseos?> GetByIdAsync(Guid id);
        Task AddAsync(ListaDeseos entity);
        void Update(ListaDeseos entity);
        Task<bool> ExistsAsync(Guid id);
    }
}