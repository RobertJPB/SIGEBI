using SIGEBI.Business.DTOs;
using SIGEBI.Domain.Enums.Seguridad;

namespace SIGEBI.Business.Interfaces.UseCases.Usuarios
{
    /// <summary>
    /// Contrato para la gestión del ciclo de vida de los usuarios:
    /// activar, desactivar, bloquear, cambiar rol y actualizar perfil.
    /// </summary>
    public interface IGestionarUsuarioUseCase
    {
        Task<IEnumerable<UsuarioDTO>> ObtenerTodosAsync();
        Task<UsuarioDTO?> ObtenerPorIdAsync(Guid id);
        Task ActivarAsync(Guid id);
        Task DesactivarAsync(Guid id, string motivo);
        Task SuspenderAsync(Guid id);
        Task BloquearAsync(Guid id, string motivo);
        Task CambiarRolAsync(Guid id, RolUsuario nuevoRol);
        Task EliminarAsync(Guid id);
        Task ActualizarImagenAsync(Guid id, string? nuevaUrl);
        Task ActualizarPerfilAsync(Guid id, string nuevoNombre, string nuevoCorreo);
    }
}
