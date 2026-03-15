using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.Interfaces.Persistance
{
    // Principio SOLID (ISP - Segregación de Interfaces):
    // Definimos un contrato pequeño y especifico.
    // Solo metodos de Préstamos para que quien lo use no tenga metodos de Usuarios o Libros mezclados.
    public interface IPrestamoRepository
    {
        Task<IEnumerable<Prestamo>> GetAllAsync();
        Task<Prestamo?> GetByIdAsync(Guid id);
        Task<IEnumerable<Prestamo>> GetByUsuarioIdAsync(Guid usuarioId);
        Task<IEnumerable<Prestamo>> GetActivosByUsuarioIdAsync(Guid usuarioId);
        Task<IEnumerable<Prestamo>> GetAtrasadosAsync();
        Task AddAsync(Prestamo entity);
        void Update(Prestamo entity);
        void Delete(Prestamo entity);
        Task<bool> ExistsAsync(Guid id);
    }
}