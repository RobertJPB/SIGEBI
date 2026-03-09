using Microsoft.EntityFrameworkCore;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Domain.Entities;
using SIGEBI.Infrastructure.Persistance.Base;

namespace SIGEBI.Infrastructure.Persistance.Repositories
{
    public class ValoracionRepository : BaseRepository<Valoracion>, IValoracionRepository
    {
        public ValoracionRepository(SIGEBIDbContext context) : base(context) { }

        public async Task<IEnumerable<Valoracion>> GetByRecursoIdAsync(Guid recursoId)
            => await _dbSet.Where(v => v.RecursoId == recursoId).ToListAsync();

        public async Task<IEnumerable<Valoracion>> GetByUsuarioIdAsync(Guid usuarioId)
            => await _dbSet.Where(v => v.UsuarioId == usuarioId).ToListAsync();

        public async Task<double> GetPromedioCalificacionAsync(Guid recursoId)
        {
            var valoraciones = await _dbSet.Where(v => v.RecursoId == recursoId).ToListAsync();
            if (!valoraciones.Any()) return 0;
            return valoraciones.Average(v => (double)v.Calificacion);
        }
    }
}