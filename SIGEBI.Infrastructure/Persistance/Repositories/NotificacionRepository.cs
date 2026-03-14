using Microsoft.EntityFrameworkCore;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Domain.Entities;
using SIGEBI.Infrastructure.Persistance.Base;

namespace SIGEBI.Infrastructure.Persistance.Repositories
{
    public class NotificacionRepository : BaseRepository<Notificacion>, INotificacionRepository
    {
        public NotificacionRepository(SIGEBIDbContext context) : base(context) { }

        public async Task<IEnumerable<Notificacion>> GetByUsuarioIdAsync(Guid usuarioId)
            => await _dbSet.Where(n => n.UsuarioId == usuarioId).ToListAsync();

        public new async Task<Notificacion?> GetByIdAsync(Guid id)
            => await _dbSet.FirstOrDefaultAsync(n => n.Id == id);

        public new async Task<bool> ExistsAsync(Guid id)
            => await _dbSet.AnyAsync(n => n.Id == id);
    }
}