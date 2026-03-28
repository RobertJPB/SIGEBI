using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Enums.Seguridad;

namespace SIGEBI.Business.Interfaces.Persistence
{
    public interface IUsuarioRepository : IBaseRepository<Usuario, Guid>
    {
        Task<Usuario?> GetByCorreoAsync(string correo);
        Task<IEnumerable<Usuario>> GetByRolAsync(RolUsuario rol);
        Task<IEnumerable<Usuario>> GetActivosAsync();
    }
}
