using Microsoft.EntityFrameworkCore;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Domain.Entities;
using SIGEBI.Infrastructure.Persistence.Base;

namespace SIGEBI.Infrastructure.Persistence.Repositories
{
    public class AuditoriaRepository : BaseRepository<Auditoria>, IAuditoriaRepository
    {
        public AuditoriaRepository(SIGEBIDbContext context) : base(context) { }
        
        public new async Task<IEnumerable<Auditoria>> GetAllAsync()
        {
            // Traemos todo el historial ordenado por fecha de lo más nuevo a lo viejo
            // Incluimos al Usuario para mostrar su nombre en la tabla
            return await _dbSet
                .Include(a => a.Usuario)
                .OrderByDescending(a => a.FechaRegistro)
                .ToListAsync();
        }

        public new async Task<Auditoria?> GetByIdAsync(int id)
            => await _dbSet.Include(a => a.Usuario).FirstOrDefaultAsync(a => a.Id == id);

        public new async Task<bool> ExistsAsync(int id)
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