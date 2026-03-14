using SIGEBI.Domain.Entities.Recursos;

namespace SIGEBI.Business.Interfaces.Persistance
{
    // Principio SOLID (ISP - Segregación de Interfaces):
    // Interfaz dedicada solo a la gestion de recursos bibliograficos.
    public interface IRecursoRepository
    {
        Task<IEnumerable<RecursoBibliografico>> GetAllAsync();
        Task<RecursoBibliografico?> GetByIdAsync(Guid id);
        Task<IEnumerable<RecursoBibliografico>> GetByCategoriaAsync(int categoriaId);
        Task<IEnumerable<RecursoBibliografico>> GetDisponiblesAsync();
        Task<IEnumerable<RecursoBibliografico>> BuscarPorTituloAsync(string titulo);
        Task<IEnumerable<Libro>> GetLibrosAsync();
        Task<IEnumerable<Revista>> GetRevistasAsync();
        Task<IEnumerable<Documento>> GetDocumentosAsync();
        Task AddAsync(RecursoBibliografico entity);
        void Update(RecursoBibliografico entity);
        void Delete(RecursoBibliografico entity);
        Task<bool> ExistsAsync(Guid id);
    }
}