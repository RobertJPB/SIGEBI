using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace SIGEBI.Services
{
    // DelegatingHandler que intercepta cada petición saliente de ApiService
    // y le inyecta el token de autorización JWT si existe en la sesión activa.
    public class AuthHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(SessionService.Token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", SessionService.Token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
