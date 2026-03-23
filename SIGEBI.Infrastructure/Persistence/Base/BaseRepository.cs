using Microsoft.EntityFrameworkCore;
using SIGEBI.Infrastructure.Persistence;

namespace SIGEBI.Infrastructure.Persistence.Base
{
    // Esta clase base tiene todo el CRUD basico. Esta cerrada a modificaciones (no hace falta tocarla)
    // pero abierta a extension (creamos un UsuarioRepository que hereda de aca y le suma metodos extra).
    public class BaseRepository<T> where T : class
    {
        protected readonly SIGEBIDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(SIGEBIDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            // Trae todo de la tabla, ojo con tablas gigantes!
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(Guid id)
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
