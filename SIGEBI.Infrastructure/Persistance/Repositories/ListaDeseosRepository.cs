using Microsoft.EntityFrameworkCore;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Domain.Entities;
using SIGEBI.Infrastructure.Persistance.Base;

namespace SIGEBI.Infrastructure.Persistance.Repositories
{
    public class ListaDeseosRepository : BaseRepository<ListaDeseos>, IListaDeseosRepository
    {
        public ListaDeseosRepository(SIGEBIDbContext context) : base(context) { }

        public async Task<ListaDeseos?> GetByUsuarioIdAsync(Guid usuarioId)
        {
            // Necesitamos meter el Include para traer la relacion con Recursos (sino EF te la trae vacia)
            return await _dbSet
                .Include(l => l.Recursos)
                .FirstOrDefaultAsync(l => l.UsuarioId == usuarioId);
        }

        public new async Task<ListaDeseos?> GetByIdAsync(Guid id)
            => await _dbSet
                .Include(l => l.Recursos)
                .FirstOrDefaultAsync(l => l.Id == id);

        public new async Task<bool> ExistsAsync(Guid id)
            => await _dbSet.AnyAsync(l => l.Id == id);
    }
}