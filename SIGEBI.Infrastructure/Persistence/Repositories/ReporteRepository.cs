using Microsoft.EntityFrameworkCore;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Domain.Entities;
using SIGEBI.Infrastructure.Persistence.Base;

namespace SIGEBI.Infrastructure.Persistence.Repositories
{
    public class ReporteRepository : BaseRepository<Reporte, Guid>, IReporteRepository
    {
        public ReporteRepository(SIGEBIDbContext context) : base(context) { }

        public async Task<IEnumerable<Reporte>> GetUltimosReportesAsync(int count)
        {
            return await _dbSet
                .OrderByDescending(r => r.FechaGeneracion)
                .Take(count)
                .ToListAsync();
        }
    }
}
