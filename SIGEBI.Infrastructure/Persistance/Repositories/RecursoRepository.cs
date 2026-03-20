using Microsoft.EntityFrameworkCore;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Domain.Entities.Recursos;
using SIGEBI.Domain.Enums.Biblioteca;
using SIGEBI.Infrastructure.Persistance.Base;

namespace SIGEBI.Infrastructure.Persistance.Repositories
{
    // El repo devuelve IEnumerable<RecursoBibliografico>, pero por detras Entity Framework nos puede 
    // devolver Libros, Revistas o Documentos mezclados, y el sistema sigue funcionando igual.
    public class RecursoRepository : BaseRepository<RecursoBibliografico>, IRecursoRepository
    {
        public RecursoRepository(SIGEBIDbContext context) : base(context) { }

        public override async Task<IEnumerable<RecursoBibliografico>> GetAllAsync()
            => await _dbSet.Include(r => r.Categoria).ToListAsync();

        public async Task<IEnumerable<RecursoBibliografico>> GetByCategoriaAsync(int categoriaId)
            => await _dbSet
                .Include(r => r.Categoria)
                .Where(r => r.IdCategoria == categoriaId)
                .ToListAsync();

        public async Task<IEnumerable<RecursoBibliografico>> GetDisponiblesAsync()
        {
            // Solo trae los recursos que todavia tienen stock y no estan inactivos
            return await _dbSet
                .Include(r => r.Categoria)
                .Where(r => r.Estado == EstadoRecurso.Disponible)
                .ToListAsync();
        }

        public async Task<IEnumerable<RecursoBibliografico>> BuscarPorTituloAsync(string titulo)
            => await _dbSet
                .Include(r => r.Categoria)
                .Where(r => r.Titulo.Contains(titulo))
                .ToListAsync();

        public async Task<IEnumerable<Libro>> GetLibrosAsync()
            => await _context.Set<Libro>().Include(r => r.Categoria).ToListAsync();

        public async Task<IEnumerable<Revista>> GetRevistasAsync()
            => await _context.Set<Revista>().Include(r => r.Categoria).ToListAsync();

        public async Task<IEnumerable<Documento>> GetDocumentosAsync()
            => await _context.Set<Documento>().Include(r => r.Categoria).ToListAsync();
    }
}
