using SIGEBI.Domain.Entities;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Infrastructure.Persistence.Base;

namespace SIGEBI.Infrastructure.Persistence.Repositories
{
    public class SolicitudAccesoRepository : BaseRepository<SolicitudAcceso, Guid>, ISolicitudAccesoRepository
    {
        public SolicitudAccesoRepository(SIGEBIDbContext context) : base(context) { }
    }
}
