using System.Threading.Tasks;
using Refit;
using SIGEBI.Business.DTOs;

namespace SIGEBI.Services
{
    public record LoginRequestDTO(string Correo, string Contrasena);
    public record LoginResponseDTO(string Token);

    public interface IAuthApi
    {
        [Post("/api/Auth/login")]
        Task<LoginResponseDTO> LoginAsync([Body] LoginRequestDTO request);

        [Post("/api/Auth/registrar")]
        Task RegistrarUsuarioAsync([Body] UsuarioDTO dto);
    }
}
