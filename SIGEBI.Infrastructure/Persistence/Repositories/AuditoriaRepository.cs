using Microsoft.EntityFrameworkCore;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Domain.Entities;
using SIGEBI.Infrastructure.Persistence.Base;

namespace SIGEBI.Infrastructure.Persistence.Repositories
{
    public class AuditoriaRepository : BaseRepository<Auditoria, int>, IAuditoriaRepository
    {
        public AuditoriaRepository(SIGEBIDbContext context) : base(context) { }
        
        public override async Task<IEnumerable<Auditoria>> GetAllAsync() // Todo el historial
        {
            return await _dbSet
                .Include(a => a.Usuario)
                .OrderByDescending(a => a.FechaRegistro)
                .ToListAsync();
        }

        public override async Task<Auditoria?> GetByIdAsync(int id) // Obtener por ID con Usuario
            => await _dbSet.Include(a => a.Usuario).FirstOrDefaultAsync(a => a.Id == id);

        public async Task<IEnumerable<Auditoria>> GetByUsuarioIdAsync(Guid usuarioId) // Historial de un usuario
        {
            return await _dbSet
                .Include(a => a.Usuario)
                .Where(a => a.UsuarioId == usuarioId)
                .OrderByDescending(a => a.FechaRegistro)
                .ToListAsync();
        }
    }
}
