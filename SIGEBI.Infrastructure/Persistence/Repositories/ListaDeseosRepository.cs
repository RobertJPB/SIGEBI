using Microsoft.EntityFrameworkCore;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Domain.Entities;
using SIGEBI.Infrastructure.Persistence.Base;

namespace SIGEBI.Infrastructure.Persistence.Repositories
{
    public class ListaDeseosRepository : BaseRepository<ListaDeseos>, IListaDeseosRepository
    {
        public ListaDeseosRepository(SIGEBIDbContext context) : base(context) { }

        public async Task<ListaDeseos?> GetByUsuarioIdAsync(Guid usuarioId)
        {
            // Necesitamos meter el Include para traer la relacion con Recursos (sino EF te la trae vacia)
            // Y de paso el ThenInclude para la categoria de cada recurso
            return await _dbSet
                .Include(l => l.Usuario)
                .Include(l => l.Recursos)
                    .ThenInclude(r => r.Categoria)
                .FirstOrDefaultAsync(l => l.UsuarioId == usuarioId);
        }

        public new async Task<ListaDeseos?> GetByIdAsync(Guid id)
            => await _dbSet
                .Include(l => l.Usuario)
                .Include(l => l.Recursos)
                    .ThenInclude(r => r.Categoria)
                .FirstOrDefaultAsync(l => l.Id == id);

        public new async Task<bool> ExistsAsync(Guid id)
            => await _dbSet.AnyAsync(l => l.Id == id);

        public async Task<IEnumerable<Guid>> GetUsuariosInteresadosAsync(Guid recursoId)
        {
            return await _dbSet
                .Where(l => l.Recursos.Any(r => r.Id == recursoId))
                .Select(l => l.UsuarioId)
                .ToListAsync();
        }
    }
}