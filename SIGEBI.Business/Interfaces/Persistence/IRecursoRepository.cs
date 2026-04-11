using SIGEBI.Domain.Entities.Recursos;

namespace SIGEBI.Business.Interfaces.Persistence
{
    public interface IRecursoRepository : IBaseRepository<RecursoBibliografico, Guid>
    {
        Task<IEnumerable<RecursoBibliografico>> GetByCategoriaAsync(int categoriaId);
        Task<IEnumerable<RecursoBibliografico>> GetDisponiblesAsync();
        Task<IEnumerable<RecursoBibliografico>> BuscarPorTituloAsync(string query);
        Task<IEnumerable<Libro>> GetLibrosAsync();
        Task<IEnumerable<Revista>> GetRevistasAsync();
        Task<IEnumerable<Documento>> GetDocumentosAsync();
        Task<IEnumerable<string>> GetAutoresUnicosAsync();
        Task<IEnumerable<string>> GetEditorialesUnicasAsync();
    }
}
