using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Enums.Seguridad;

namespace SIGEBI.Business.Interfaces.Persistence
{
    // Definimos un contrato pequeño y especifico para operaciones de Usuario.
    // Esto evita tener una interfaz monstruosa "IGestorDatos" obligando a las clases a implementar metodos que no usan.
    public interface IUsuarioRepository
    {
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<Usuario?> GetByIdAsync(Guid id);
        Task<Usuario?> GetByCorreoAsync(string correo);
        Task<IEnumerable<Usuario>> GetByRolAsync(RolUsuario rol);
        Task<IEnumerable<Usuario>> GetActivosAsync();
        Task AddAsync(Usuario entity);
        void Update(Usuario entity);
        void Delete(Usuario entity);
        Task<bool> ExistsAsync(Guid id);
    }
}
