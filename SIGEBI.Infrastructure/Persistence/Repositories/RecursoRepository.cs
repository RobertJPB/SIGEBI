using Microsoft.EntityFrameworkCore;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Domain.Entities.Recursos;
using SIGEBI.Domain.Enums.Biblioteca;
using SIGEBI.Infrastructure.Persistence.Base;

namespace SIGEBI.Infrastructure.Persistence.Repositories
{
 
    public class RecursoRepository : BaseRepository<RecursoBibliografico, Guid>, IRecursoRepository
    {
        public RecursoRepository(SIGEBIDbContext context) : base(context) { }

        public override async Task<IEnumerable<RecursoBibliografico>> GetAllAsync() // Obtener todos con categorías
            => await _dbSet.Include(r => r.Categoria).AsNoTracking().ToListAsync();

        public async Task<IEnumerable<RecursoBibliografico>> GetByCategoriaAsync(int categoriaId) // Filtrar por categoría
            => await _dbSet
                .AsNoTracking()
                .Include(r => r.Categoria)
                .Where(r => r.IdCategoria == categoriaId)
                .ToListAsync();

        public async Task<IEnumerable<RecursoBibliografico>> GetDisponiblesAsync() // Solo disponibles
        {
            return await _dbSet
                .AsNoTracking()
                .Include(r => r.Categoria)
                .Where(r => r.Estado == EstadoRecurso.Disponible)
                .ToListAsync();
        }

        public async Task<IEnumerable<RecursoBibliografico>> BuscarPorTituloAsync(string titulo) // Buscar por título
            => await _dbSet
                .AsNoTracking()
                .Include(r => r.Categoria)
                .Where(r => r.Titulo.Contains(titulo))
                .ToListAsync();

        public async Task<IEnumerable<Libro>> GetLibrosAsync() // Solo libros
            => await _context.Set<Libro>().Include(r => r.Categoria).AsNoTracking().ToListAsync();

        public async Task<IEnumerable<Revista>> GetRevistasAsync() // Solo revistas
            => await _context.Set<Revista>().Include(r => r.Categoria).AsNoTracking().ToListAsync();

        public async Task<IEnumerable<Documento>> GetDocumentosAsync() // Solo documentos
            => await _context.Set<Documento>().Include(r => r.Categoria).AsNoTracking().ToListAsync();
    }
}
