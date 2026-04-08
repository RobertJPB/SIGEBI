using Microsoft.EntityFrameworkCore;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Domain.Entities;
using SIGEBI.Infrastructure.Persistence.Base;

namespace SIGEBI.Infrastructure.Persistence.Repositories
{
    public class ListaDeseosRepository : BaseRepository<ListaDeseos, Guid>, IListaDeseosRepository
    {
        public ListaDeseosRepository(SIGEBIDbContext context) : base(context) { }

        public async Task<ListaDeseos?> GetByUsuarioIdAsync(Guid usuarioId) // Por usuario (con recursos)
        {
            return await _dbSet
                .Include(l => l.Usuario)
                .Include(l => l.Recursos)
                    .ThenInclude(r => r.Categoria)
                .FirstOrDefaultAsync(l => l.UsuarioId == usuarioId);
        }

        public override async Task<ListaDeseos?> GetByIdAsync(Guid id) // Por ID (con recursos)
            => await _dbSet
                .Include(l => l.Usuario)
                .Include(l => l.Recursos)
                    .ThenInclude(r => r.Categoria)
                .FirstOrDefaultAsync(l => l.Id == id);

        public async Task<IEnumerable<Guid>> GetUsuariosInteresadosAsync(Guid recursoId) // Usuarios interesados
        {
            return await _dbSet
                .Where(l => l.Recursos.Any(r => r.Id == recursoId))
                .Select(l => l.UsuarioId)
                .ToListAsync();
        }
    }
}
