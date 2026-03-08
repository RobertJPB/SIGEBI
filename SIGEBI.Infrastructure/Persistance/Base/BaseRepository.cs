using Microsoft.EntityFrameworkCore;
using SIGEBI.Infrastructure.Persistance;

namespace SIGEBI.Infrastructure.Persistance.Base
{
    public class BaseRepository<T> where T : class
    {
        protected readonly SIGEBIDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(SIGEBIDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
            => await _dbSet.ToListAsync();

        public async Task<T?> GetByIdAsync(Guid id)
            => await _dbSet.FindAsync(id);

        public async Task AddAsync(T entity)
            => await _dbSet.AddAsync(entity);

        public void Update(T entity)
            => _dbSet.Update(entity);

        public void Delete(T entity)
            => _dbSet.Remove(entity);

        public async Task<bool> ExistsAsync(Guid id)
            => await _dbSet.FindAsync(id) != null;
    }
}