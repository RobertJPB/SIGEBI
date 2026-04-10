using System.Text.Json;
using System.Threading.Tasks;

namespace SIGEBI.Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthApi _api;

        public AuthService(IAuthApi api)
        {
            _api = api;
        }

        public async Task<JsonElement> LoginAsync(string correo, string clave)
        {
            var request = new { Correo = correo, Contrasena = clave };
            return await _api.LoginAsync(request);
        }
    }
}
