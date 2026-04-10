using Microsoft.AspNetCore.Http;
using SIGEBI.Business.DTOs;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SIGEBI.Web.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuariosApi _api;

        public UsuarioService(IUsuariosApi api)
        {
            _api = api;
        }

        public async Task<UsuarioDTO> GetPerfilAsync(string token)
        {
            return await _api.GetPerfilAsync(token);
        }

        public async Task ActualizarDatosAsync(string nombre, string correo, string token)
        {
            var request = new { Nombre = nombre, Correo = correo };
            await _api.ActualizarDatosAsync(request, token);
        }

        public async Task<string> ActualizarFotoAsync(IFormFile foto, string token)
        {
            // Nota: Refit maneja multipart si se define correctamente, 
            // pero para IFormFile a veces es más directo usar MultipartFormDataContent si no se quiere cambiar la interfaz.
            // Dada la simplicidad del requerimiento y para mantener la pureza de ISigebiApi, 
            // simularemos el envío aunque en un entorno real usaríamos los atributos [Multipart] y [AliasAs] de Refit.
            
            using var content = new MultipartFormDataContent();
            using var fileStream = foto.OpenReadStream();
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(foto.ContentType);
            content.Add(fileContent, "foto", foto.FileName);

            // Refit soporta enviar un Stream directamente si se anotara con [Body] y se gestionara el boundary,
            // pero el endpoint espera MultipartFormDataContent. 
            // Por simplicidad en esta fase académica, pasaremos el objeto.
            return await _api.ActualizarFotoAsync(content, token);
        }
    }
}
