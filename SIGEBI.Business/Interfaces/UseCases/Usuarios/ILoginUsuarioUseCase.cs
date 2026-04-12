using SIGEBI.Business.DTOs;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.Interfaces.UseCases.Usuarios
{
    public interface ILoginUsuarioUseCase
    {
        Task<Usuario?> EjecutarAsync(string correo, string password);
    }
}

