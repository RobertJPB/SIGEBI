using Microsoft.EntityFrameworkCore;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Infrastructure.Persistence;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Infrastructure.Persistence.Base
{
    // Esta clase base tiene todo el CRUD basico. 
    public class BaseRepository<T, TId> : IBaseRepository<T, TId> where T : class
    {
        protected readonly SIGEBIDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(SIGEBIDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync() // Obtener todo
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(TId id) // Obtener por ID
            => await _dbSet.FindAsync(id);

        public async Task AddAsync(T entity) // Agregar nuevo
            => await _dbSet.AddAsync(entity);

        public void Update(T entity) // Actualizar existente
            => _dbSet.Update(entity);

        public void Delete(T entity) // Eliminar o Borrado Logico
        {
            if (entity is IDesactivable desactivableEntity)
            {
                desactivableEntity.Desactivar("Borrado lógico desde repositorio base.");
                _dbSet.Update(entity);
            }
            else
            {
                _dbSet.Remove(entity);
            }
        }

        public void HardDelete(T entity) // Borrado fisico permanente
        {
            _dbSet.Remove(entity);
        }

        public async Task<bool> ExistsAsync(TId id) // Verificar existencia por ID sin cargar entidad
            => await _dbSet.AnyAsync(e => EF.Property<TId>(e, "Id")!.Equals(id));
    }
}
