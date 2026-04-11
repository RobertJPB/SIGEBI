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

        public override async Task<IEnumerable<RecursoBibliografico>> GetAllAsync()
            => await _dbSet.Include(r => r.Categoria).Include(r => r.UsuarioCreador)
                .OrderByDescending(r => r.FechaCreacion)
                .AsNoTracking().ToListAsync();

        public async Task<IEnumerable<RecursoBibliografico>> GetByCategoriaAsync(int categoriaId)
            => await _dbSet
                .AsNoTracking()
                .Include(r => r.Categoria)
                .Include(r => r.UsuarioCreador)
                .Where(r => r.IdCategoria == categoriaId)
                .OrderByDescending(r => r.FechaCreacion)
                .ToListAsync();

        public override async Task<RecursoBibliografico?> GetByIdAsync(Guid id)
            => await _dbSet.Include(r => r.Categoria).Include(r => r.UsuarioCreador).FirstOrDefaultAsync(r => r.Id == id);

        public async Task<IEnumerable<RecursoBibliografico>> GetDisponiblesAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(r => r.Categoria)
                .Include(r => r.UsuarioCreador)
                .Where(r => r.Estado == EstadoRecurso.Disponible)
                .OrderByDescending(r => r.FechaCreacion)
                .ToListAsync();
        }

        public async Task<IEnumerable<RecursoBibliografico>> BuscarPorTituloAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return await GetAllAsync();

            var queryLower = query.ToLower();
            return await _dbSet
                .AsNoTracking()
                .Include(r => r.Categoria)
                .Include(r => r.UsuarioCreador)
                .Where(r => r.Titulo.ToLower().Contains(queryLower) || 
                            r.Autor.ToLower().Contains(queryLower))
                .ToListAsync();
        }

        public async Task<IEnumerable<Libro>> GetLibrosAsync()
            => await _context.Set<Libro>().Include(r => r.Categoria).Include(r => r.UsuarioCreador)
                .OrderByDescending(r => r.FechaCreacion).AsNoTracking().ToListAsync();

        public async Task<IEnumerable<Revista>> GetRevistasAsync()
            => await _context.Set<Revista>().Include(r => r.Categoria).Include(r => r.UsuarioCreador)
                .OrderByDescending(r => r.FechaCreacion).AsNoTracking().ToListAsync();

        public async Task<IEnumerable<Documento>> GetDocumentosAsync()
            => await _context.Set<Documento>().Include(r => r.Categoria).Include(r => r.UsuarioCreador)
                .OrderByDescending(r => r.FechaCreacion).AsNoTracking().ToListAsync();

        public async Task<IEnumerable<string>> GetAutoresUnicosAsync()
        {
            return await _dbSet.AsNoTracking()
                .Select(r => r.Autor)
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .Distinct()
                .OrderBy(a => a)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetEditorialesUnicasAsync()
        {
            var editorialesLibros = await _context.Set<Libro>().AsNoTracking()
                .Select(l => l.Editorial)
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .Distinct()
                .ToListAsync();

            var editorialesRevistas = await _context.Set<Revista>().AsNoTracking()
                .Select(r => r.Editorial)
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .Distinct()
                .ToListAsync();

            return editorialesLibros.Union(editorialesRevistas!)
                .OrderBy(e => e!)
                .ToList();
        }
    }
}

