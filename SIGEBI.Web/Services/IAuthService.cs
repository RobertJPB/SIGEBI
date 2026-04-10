using System.Text.Json;
using System.Threading.Tasks;

namespace SIGEBI.Web.Services
{
    public interface IAuthService
    {
        Task<JsonElement> LoginAsync(string correo, string clave);
    }
}
