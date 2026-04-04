using SIGEBI.Business.DTOs;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.Interfaces.UseCases.Usuarios
{
    /// <summary>
    /// Contrato para el caso de uso de autenticación de usuarios.
    /// Retorna null cuando las credenciales son incorrectas.
    /// </summary>
    public interface ILoginUsuarioUseCase
    {
        Task<Usuario?> EjecutarAsync(string correo, string password);
    }
}
