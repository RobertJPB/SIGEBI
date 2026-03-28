using Microsoft.EntityFrameworkCore;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Domain.Entities;
using SIGEBI.Infrastructure.Persistence.Base;

namespace SIGEBI.Infrastructure.Persistence.Repositories
{
    public class NotificacionRepository : BaseRepository<Notificacion, Guid>, INotificacionRepository
    {
        public NotificacionRepository(SIGEBIDbContext context) : base(context) { }

        public async Task<IEnumerable<Notificacion>> GetByUsuarioIdAsync(Guid usuarioId) // Notificaciones del usuario
        {
            return await _dbSet
                .AsNoTracking()
                .Where(n => n.UsuarioId == usuarioId)
                .OrderByDescending(n => n.Fecha)
                .ToListAsync();
        }

        public async Task<int> GetCantPendientesAsync(Guid usuarioId) // Cantidad de no leídas
        {
            return await _dbSet
                .CountAsync(n => n.UsuarioId == usuarioId && n.Estado == SIGEBI.Domain.Enums.Operacion.EstadoNotificacion.NoLeida);
        }
    }
}