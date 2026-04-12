using SIGEBI.Business.DTOs;

namespace SIGEBI.Business.Interfaces.UseCases.Usuarios
{
    public interface IRegistrarUsuarioUseCase
    {
        Task<Guid> EjecutarAsync(UsuarioDTO dto);
    }
}

