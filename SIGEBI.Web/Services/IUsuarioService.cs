using Microsoft.AspNetCore.Http;
using SIGEBI.Business.DTOs;
using System.Threading.Tasks;

namespace SIGEBI.Web.Services
{
    public interface IUsuarioService
    {
        Task<UsuarioDTO> GetPerfilAsync(string token);
        Task ActualizarDatosAsync(string nombre, string correo, string token);
        Task<string> ActualizarFotoAsync(IFormFile foto, string token);
    }
}
