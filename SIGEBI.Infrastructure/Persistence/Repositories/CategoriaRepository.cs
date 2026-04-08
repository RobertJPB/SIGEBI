using Microsoft.EntityFrameworkCore;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Enums.Biblioteca;
using SIGEBI.Infrastructure.Persistence.Base;

namespace SIGEBI.Infrastructure.Persistence.Repositories
{
    public class CategoriaRepository : BaseRepository<Categoria, int>, ICategoriaRepository
    {
        public CategoriaRepository(SIGEBIDbContext context) : base(context) 
        { 
        }

        public async Task<Categoria?> GetByNombreAsync(string nombre) // Buscar por nombre
            => await _dbSet.FirstOrDefaultAsync(c => c.Nombre == nombre);

        public async Task<IEnumerable<Categoria>> GetActivasAsync() // Solo activas
        {
            return await _dbSet.Where(c => c.Estado == EstadoCategoria.Activa).ToListAsync();
        }
    }
}
