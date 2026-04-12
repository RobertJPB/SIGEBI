using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.Interfaces.Persistence
{
    public interface IReporteRepository : IBaseRepository<Reporte, Guid>
    {
        Task<IEnumerable<Reporte>> GetUltimosReportesAsync(int count);
    }
}
