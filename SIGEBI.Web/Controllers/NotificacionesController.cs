using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SIGEBI.Web.Controllers
{
    public class NotificacionViewModel
    {
        public Guid Id { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public bool Leida { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    public class NotificacionesIndexViewModel
    {
        public List<NotificacionViewModel> Notificaciones { get; set; } = new();
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class NotificacionesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public NotificacionesController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToAction("Login", "Auth");

            var usuarioId = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrWhiteSpace(usuarioId))
                return RedirectToAction("Login", "Auth");

            var model = new NotificacionesIndexViewModel();

            try
            {
                var client = _httpClientFactory.CreateClient("SIGEBIAPI");
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync($"api/Notificaciones/usuario/{usuarioId}");

                if (!response.IsSuccessStatusCode)
                {
                    model.ErrorMessage = "No se pudieron cargar las notificaciones.";
                    return View(model);
                }

                var json = await response.Content.ReadAsStringAsync();
                model.Notificaciones = JsonSerializer.Deserialize<List<NotificacionViewModel>>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? new List<NotificacionViewModel>();
            }
            catch
            {
                model.ErrorMessage = "Error al conectar con el servidor.";
            }

            return View(model);
        }
    }
}
