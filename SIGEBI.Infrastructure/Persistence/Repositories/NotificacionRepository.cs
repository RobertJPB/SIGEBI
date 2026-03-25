using Microsoft.EntityFrameworkCore;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Domain.Entities;
using SIGEBI.Infrastructure.Persistence.Base;

namespace SIGEBI.Infrastructure.Persistence.Repositories
{
    public class NotificacionRepository : BaseRepository<Notificacion>, INotificacionRepository
    {
        public NotificacionRepository(SIGEBIDbContext context) : base(context) { }

        public async Task<IEnumerable<Notificacion>> GetByUsuarioIdAsync(Guid usuarioId)
            => await _dbSet.Where(n => n.UsuarioId == usuarioId).ToListAsync();

        public async Task<int> GetCantPendientesAsync(Guid usuarioId)
            => await _dbSet.CountAsync(n => n.UsuarioId == usuarioId && n.Estado == SIGEBI.Domain.Enums.Operacion.EstadoNotificacion.NoLeida);

        public new async Task<Notificacion?> GetByIdAsync(Guid id)
            => await _dbSet.FirstOrDefaultAsync(n => n.Id == id);

        public new async Task<bool> ExistsAsync(Guid id)
            => await _dbSet.AnyAsync(n => n.Id == id);
    }
}