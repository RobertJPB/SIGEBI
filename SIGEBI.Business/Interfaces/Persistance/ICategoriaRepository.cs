using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.Interfaces.Persistance
{
    public interface ICategoriaRepository
    {
        Task<IEnumerable<Categoria>> GetAllAsync();
        Task<Categoria?> GetByIdAsync(int id);
        Task<Categoria?> GetByNombreAsync(string nombre);
        Task<IEnumerable<Categoria>> GetActivasAsync();
        Task AddAsync(Categoria entity);
        void Update(Categoria entity);
        void Delete(Categoria entity);
        Task<bool> ExistsAsync(int id);
    }
}