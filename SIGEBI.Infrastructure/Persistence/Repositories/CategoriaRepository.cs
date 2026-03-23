using Microsoft.EntityFrameworkCore;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Enums.Biblioteca;
using SIGEBI.Infrastructure.Persistence.Base;


namespace SIGEBI.Infrastructure.Persistence.Repositories
{
    public class CategoriaRepository : BaseRepository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(SIGEBIDbContext context) : base(context) { }

        public async Task<Categoria?> GetByIdAsync(int id)
            => await _dbSet.FindAsync(id);

        public async Task<bool> ExistsAsync(int id)
            => await _dbSet.FindAsync(id) != null;

        public async Task<Categoria?> GetByNombreAsync(string nombre)
            => await _dbSet.FirstOrDefaultAsync(c => c.Nombre == nombre);

        public async Task<IEnumerable<Categoria>> GetActivasAsync()
        {
            // Ojo: Solo listamos las categorias que esten activas para que no salgan cosas borradas
            return await _dbSet.Where(c => c.Estado == EstadoCategoria.Activa).ToListAsync();
        }
    }
}