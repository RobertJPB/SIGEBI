using SIGEBI.Business.DTOs;

namespace SIGEBI.Business.Interfaces.UseCases.Usuarios
{
    /// <summary>
    /// Contrato para el registro de nuevos usuarios en el sistema.
    /// </summary>
    public interface IRegistrarUsuarioUseCase
    {
        Task<Guid> EjecutarAsync(UsuarioDTO dto);
    }
}
