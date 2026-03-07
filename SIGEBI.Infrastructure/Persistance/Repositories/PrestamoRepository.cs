using Microsoft.EntityFrameworkCore;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Enums.Biblioteca;
using SIGEBI.Infrastructure.Persistance.Base;

namespace SIGEBI.Infrastructure.Persistance.Repositories
{
    public class PrestamoRepository : BaseRepository<Prestamo>, IPrestamoRepository
    {
        public PrestamoRepository(SIGEBIDbContext context) : base(context) { }

        public async Task<IEnumerable<Prestamo>> GetByUsuarioIdAsync(Guid usuarioId)
            => await _dbSet.Where(p => p.UsuarioId == usuarioId).ToListAsync();

        public async Task<IEnumerable<Prestamo>> GetActivosByUsuarioIdAsync(Guid usuarioId)
            => await _dbSet.Where(p => p.UsuarioId == usuarioId &&
                (p.EstadoActual == EstadoPrestamo.Activo ||
                 p.EstadoActual == EstadoPrestamo.Atrasado))
                .ToListAsync();

        public async Task<IEnumerable<Prestamo>> GetAtrasadosAsync()
            => await _dbSet.Where(p => p.EstadoActual == EstadoPrestamo.Atrasado)
                .ToListAsync();
    }
}