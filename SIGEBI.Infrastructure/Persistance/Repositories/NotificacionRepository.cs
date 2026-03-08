using Microsoft.EntityFrameworkCore;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Enums.Operacion;
using SIGEBI.Infrastructure.Persistance.Base;

namespace SIGEBI.Infrastructure.Persistance.Repositories
{
    public class NotificacionRepository : BaseRepository<Notificacion>, INotificacionRepository
    {
        public NotificacionRepository(SIGEBIDbContext context) : base(context) { }

        public async Task<IEnumerable<Notificacion>> GetByUsuarioIdAsync(Guid usuarioId)
            => await _dbSet.Where(n => n.UsuarioId == usuarioId).ToListAsync();
    }
}