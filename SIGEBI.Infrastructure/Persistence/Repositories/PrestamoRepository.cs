using Microsoft.EntityFrameworkCore;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Enums.Biblioteca;
using SIGEBI.Infrastructure.Persistence.Base;

namespace SIGEBI.Infrastructure.Persistence.Repositories
{
    public class PrestamoRepository : BaseRepository<Prestamo, Guid>, IPrestamoRepository
    {
        public PrestamoRepository(SIGEBIDbContext context) : base(context) { }

        public override async Task<IEnumerable<Prestamo>> GetAllAsync() // Todos con Usuario y Recurso
            => await _dbSet.Include(p => p.Usuario).Include(p => p.Recurso).ToListAsync();

        public async Task<IEnumerable<Prestamo>> GetByUsuarioIdAsync(Guid usuarioId) // Préstamos por usuario
            => await _dbSet.Include(p => p.Usuario).Include(p => p.Recurso)
                .Where(p => p.UsuarioId == usuarioId).ToListAsync();

        public async Task<IEnumerable<Prestamo>> GetActivosByUsuarioIdAsync(Guid usuarioId) // Préstamos activos/atrasados por usuario
            => await _dbSet.Include(p => p.Usuario).Include(p => p.Recurso)
                .Where(p => p.UsuarioId == usuarioId &&
                (p.EstadoActual == EstadoPrestamo.Activo ||
                 p.EstadoActual == EstadoPrestamo.Atrasado))
                .ToListAsync();

        public async Task<IEnumerable<Prestamo>> GetAtrasadosAsync() // Todos los atrasados
        {
            return await _dbSet.Include(p => p.Usuario).Include(p => p.Recurso)
                .Where(p => p.EstadoActual == EstadoPrestamo.Atrasado)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prestamo>> GetActivosByRecursoIdAsync(Guid recursoId)
        {
            return await _dbSet
                .Where(p => p.RecursoId == recursoId && 
                            (p.EstadoActual == EstadoPrestamo.Activo || p.EstadoActual == EstadoPrestamo.Atrasado))
                .ToListAsync();
        }
    }
}
