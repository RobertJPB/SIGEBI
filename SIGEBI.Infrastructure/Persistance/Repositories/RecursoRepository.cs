using Microsoft.EntityFrameworkCore;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Domain.Entities.Recursos;
using SIGEBI.Domain.Enums.Biblioteca;
using SIGEBI.Infrastructure.Persistance.Base;

namespace SIGEBI.Infrastructure.Persistance.Repositories
{
    public class RecursoRepository : BaseRepository<RecursoBibliografico>, IRecursoRepository
    {
        public RecursoRepository(SIGEBIDbContext context) : base(context) { }

        public async Task<IEnumerable<RecursoBibliografico>> GetByCategoriaAsync(int categoriaId)
            => await _dbSet.Where(r => r.IdCategoria == categoriaId).ToListAsync();

        public async Task<IEnumerable<RecursoBibliografico>> GetDisponiblesAsync()
            => await _dbSet.Where(r => r.Estado == EstadoRecurso.Disponible).ToListAsync();

        public async Task<IEnumerable<RecursoBibliografico>> BuscarPorTituloAsync(string titulo)
            => await _dbSet.Where(r => r.Titulo.Contains(titulo)).ToListAsync();

        public async Task<IEnumerable<Libro>> GetLibrosAsync()
            => await _context.Set<Libro>().ToListAsync();

        public async Task<IEnumerable<Revista>> GetRevistasAsync()
            => await _context.Set<Revista>().ToListAsync();

        public async Task<IEnumerable<Documento>> GetDocumentosAsync()
            => await _context.Set<Documento>().ToListAsync();
    }
}