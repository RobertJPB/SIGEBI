using Microsoft.EntityFrameworkCore;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Enums.Operacion;
using SIGEBI.Infrastructure.Persistence.Base;

namespace SIGEBI.Infrastructure.Persistence.Repositories
{
    public class PenalizacionRepository : BaseRepository<Penalizacion, Guid>, IPenalizacionRepository
    {
        public PenalizacionRepository(SIGEBIDbContext context) : base(context) { }

        public override async Task<IEnumerable<Penalizacion>> GetAllAsync() // Todas con Usuario
            => await _dbSet.Include(p => p.Usuario).ToListAsync();

        public async Task<IEnumerable<Penalizacion>> GetByUsuarioIdAsync(Guid usuarioId) // Penalizaciones de un usuario
            => await _dbSet.Include(p => p.Usuario).Where(p => p.UsuarioId == usuarioId).ToListAsync();

        public async Task<IEnumerable<Penalizacion>> GetActivasAsync() // Solo activas
        {
            return await _dbSet.Include(p => p.Usuario).Where(p => p.Estado == EstadoPenalizacion.Activa).ToListAsync();
        }

        public override async Task<Penalizacion?> GetByIdAsync(Guid id) // Obtener por ID con Usuario
            => await _dbSet.Include(p => p.Usuario).FirstOrDefaultAsync(p => p.Id == id);

        public async Task<Penalizacion?> GetByPrestamoIdAsync(Guid prestamoId) // Buscar por préstamo
            => await _dbSet.FirstOrDefaultAsync(p => p.PrestamoId == prestamoId && p.Estado == EstadoPenalizacion.Activa);
    }
}