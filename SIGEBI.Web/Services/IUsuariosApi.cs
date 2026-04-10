using System.Threading.Tasks;
using Refit;
using SIGEBI.Business.DTOs;

namespace SIGEBI.Web.Services
{
    public interface IUsuariosApi
    {
        [Get("/api/Usuarios/perfil")]
        Task<UsuarioDTO> GetPerfilAsync([Header("Authorization")] string token);

        [Post("/api/Usuarios/perfil/foto")]
        Task<string> ActualizarFotoAsync([Body] object multipartData, [Header("Authorization")] string token);

        [Put("/api/Usuarios/perfil")]
        Task ActualizarDatosAsync([Body] object request, [Header("Authorization")] string token);
    }
}
