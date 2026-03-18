using Microsoft.EntityFrameworkCore;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Domain.Entities;
using SIGEBI.Infrastructure.Persistance.Base;

namespace SIGEBI.Infrastructure.Persistance.Repositories
{
    public class AuditoriaRepository : BaseRepository<Auditoria>, IAuditoriaRepository
    {
        public AuditoriaRepository(SIGEBIDbContext context) : base(context) { }

        public async Task<Auditoria?> GetByIdAsync(int id)
            => await _dbSet.Include(a => a.Usuario).FirstOrDefaultAsync(a => a.Id == id);

        public async Task<bool> ExistsAsync(int id)
            => await _dbSet.AnyAsync(a => a.Id == id);

        public async Task<IEnumerable<Auditoria>> GetByUsuarioIdAsync(Guid usuarioId)
        {
            // Traemos todo el historial de un usuario ordenado por fecha de lo más nuevo a lo viejo
            return await _dbSet
                .Include(a => a.Usuario)
                .Where(a => a.UsuarioId == usuarioId)
                .OrderByDescending(a => a.FechaRegistro)
                .ToListAsync();
        }
    }
}