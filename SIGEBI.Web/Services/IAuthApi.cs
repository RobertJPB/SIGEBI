using System.Text.Json;
using System.Threading.Tasks;
using Refit;

namespace SIGEBI.Web.Services
{
    public interface IAuthApi
    {
        [Post("/api/Auth/login")]
        Task<JsonElement> LoginAsync([Body] object request);
    }
}
