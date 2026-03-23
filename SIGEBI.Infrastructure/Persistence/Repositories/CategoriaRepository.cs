using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Enums.Biblioteca;
using SIGEBI.Infrastructure.Persistence.Base;


namespace SIGEBI.Infrastructure.Persistence.Repositories
{
    public class CategoriaRepository : BaseRepository<Categoria>, ICategoriaRepository
    {
        private readonly IMemoryCache _cache;

        public CategoriaRepository(SIGEBIDbContext context, IMemoryCache cache) : base(context) 
        { 
            _cache = cache;
        }

        public override async Task<IEnumerable<Categoria>> GetAllAsync()
        {
            if (!_cache.TryGetValue("AllCategorias", out IEnumerable<Categoria>? categorias))
            {
                categorias = await base.GetAllAsync();
                _cache.Set("AllCategorias", categorias, TimeSpan.FromMinutes(15));
            }
            return categorias!;
        }

        public async Task<Categoria?> GetByIdAsync(int id)
            => await _dbSet.FindAsync(id);

        public async Task<bool> ExistsAsync(int id)
            => await _dbSet.FindAsync(id) != null;

        public async Task<Categoria?> GetByNombreAsync(string nombre)
            => await _dbSet.FirstOrDefaultAsync(c => c.Nombre == nombre);

        public async Task<IEnumerable<Categoria>> GetActivasAsync()
        {
            if (!_cache.TryGetValue("ActiveCategorias", out IEnumerable<Categoria>? categorias))
            {
                categorias = await _dbSet.Where(c => c.Estado == EstadoCategoria.Activa).ToListAsync();
                _cache.Set("ActiveCategorias", categorias, TimeSpan.FromMinutes(15));
            }
            return categorias!;
        }
    }
}