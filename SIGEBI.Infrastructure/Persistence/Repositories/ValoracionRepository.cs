using Microsoft.EntityFrameworkCore;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Domain.Entities;
using SIGEBI.Infrastructure.Persistence.Base;

namespace SIGEBI.Infrastructure.Persistence.Repositories
{
    public class ValoracionRepository : BaseRepository<Valoracion, Guid>, IValoracionRepository
    {
        public ValoracionRepository(SIGEBIDbContext context) : base(context) { }

        public async Task<IEnumerable<Valoracion>> GetByRecursoIdAsync(Guid recursoId) // Valoraciones de un recurso
            => await _dbSet.Include(v => v.Usuario).Where(v => v.RecursoId == recursoId).ToListAsync();

        public async Task<IEnumerable<Valoracion>> GetByUsuarioIdAsync(Guid usuarioId) // Valoraciones por usuario
            => await _dbSet.Include(v => v.Usuario).Where(v => v.UsuarioId == usuarioId).ToListAsync();

        public async Task<double> GetPromedioCalificacionAsync(Guid recursoId) // Promedio de estrellas
        {
            var valoraciones = await _dbSet.Where(v => v.RecursoId == recursoId).ToListAsync();
            
            if (!valoraciones.Any()) return 0;
            
            return valoraciones.Average(v => (double)v.Calificacion);
        }
    }
}