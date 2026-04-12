using SIGEBI.Business.DTOs;
using SIGEBI.Domain.Enums.Seguridad;

namespace SIGEBI.Business.Interfaces.UseCases.Usuarios
{
    public interface IGestionarUsuarioUseCase
    {
        Task<IEnumerable<UsuarioDTO>> ObtenerTodosAsync();
        Task<UsuarioDTO?> ObtenerPorIdAsync(Guid id);
        Task ActivarAsync(Guid id);
        Task DesactivarAsync(Guid id, string motivo);
        Task SuspenderAsync(Guid id, string motivo);
        Task BloquearAsync(Guid id, string motivo);
        Task CambiarRolAsync(Guid id, RolUsuario nuevoRol);
        Task EliminarAsync(Guid id);
        Task ActualizarImagenAsync(Guid id, string? nuevaUrl);
        Task ActualizarPerfilAsync(Guid id, string nuevoNombre, string nuevoCorreo);
    }
}

