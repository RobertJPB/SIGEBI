using SIGEBI.Business.Interfaces;

namespace SIGEBI.Infrastructure.Persistance
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SIGEBIDbContext _context;

        public UnitOfWork(SIGEBIDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}