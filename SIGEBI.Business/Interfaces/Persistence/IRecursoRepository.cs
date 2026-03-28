using SIGEBI.Domain.Entities.Recursos;

namespace SIGEBI.Business.Interfaces.Persistence
{
    public interface IRecursoRepository : IBaseRepository<RecursoBibliografico, Guid>
    {
        Task<IEnumerable<RecursoBibliografico>> GetByCategoriaAsync(int categoriaId);
        Task<IEnumerable<RecursoBibliografico>> GetDisponiblesAsync();
        Task<IEnumerable<RecursoBibliografico>> BuscarPorTituloAsync(string titulo);
        Task<IEnumerable<Libro>> GetLibrosAsync();
        Task<IEnumerable<Revista>> GetRevistasAsync();
        Task<IEnumerable<Documento>> GetDocumentosAsync();
    }
}
