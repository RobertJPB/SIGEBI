using Microsoft.EntityFrameworkCore;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Enums.Operacion;
using SIGEBI.Infrastructure.Persistance.Base;

namespace SIGEBI.Infrastructure.Persistance.Repositories
{
    public class PenalizacionRepository : BaseRepository<Penalizacion>, IPenalizacionRepository
    {
        public PenalizacionRepository(SIGEBIDbContext context) : base(context) { }

        public override async Task<IEnumerable<Penalizacion>> GetAllAsync()
            => await _dbSet.Include(p => p.Usuario).ToListAsync();

        public async Task<IEnumerable<Penalizacion>> GetByUsuarioIdAsync(Guid usuarioId)
            => await _dbSet.Include(p => p.Usuario).Where(p => p.UsuarioId == usuarioId).ToListAsync();

        public async Task<IEnumerable<Penalizacion>> GetActivasAsync()
        {
            // Trae todas las multas que todavia no se vencieron
            return await _dbSet.Include(p => p.Usuario).Where(p => p.Estado == EstadoPenalizacion.Activa).ToListAsync();
        }

        public new async Task<Penalizacion?> GetByIdAsync(Guid id)
            => await _dbSet.Include(p => p.Usuario).FirstOrDefaultAsync(p => p.Id == id);

        public new async Task<bool> ExistsAsync(Guid id)
            => await _dbSet.AnyAsync(p => p.Id == id);
    }
}