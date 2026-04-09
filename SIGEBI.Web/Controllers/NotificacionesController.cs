using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using SIGEBI.Web.Helpers;

namespace SIGEBI.Web.Controllers
{
    public class NotificacionViewModel
    {
        public Guid Id { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public bool Leida => Estado == "Leida";
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
            var rolStr = HttpContext.Session.GetString("UsuarioRol");
            if (string.IsNullOrWhiteSpace(usuarioId) || string.IsNullOrWhiteSpace(rolStr))
                return RedirectToAction("Login", "Auth");

            var model = new NotificacionesIndexViewModel();

            try
            {
                var client = _httpClientFactory.CreateClient("SIGEBIAPI");
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                // Si es Admin o Bibliotecario, cargamos TODAS las notificaciones del sistema
                bool esAdmin = rolStr == "Administrador" || rolStr == "Bibliotecario";
                var endpoint = esAdmin ? "api/Notificaciones" : $"api/Notificaciones/usuario/{usuarioId}";

                var response = await client.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    model.ErrorMessage = "No se pudieron cargar las notificaciones.";
                    return View(model);
                }

                var json = await response.Content.ReadAsStringAsync();
                var list = JsonSerializer.Deserialize<List<NotificacionViewModel>>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? new List<NotificacionViewModel>();

                // Ordenamos por fecha descendente (más nuevas primero)
                model.Notificaciones = list.OrderByDescending(n => n.Fecha).ToList();
            }
            catch
            {
                model.ErrorMessage = "Error al conectar con el servidor.";
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> MarcarLeida(Guid id)
        {
            try
            {
                var token = HttpContext.Session.GetString("JwtToken");
                if (string.IsNullOrWhiteSpace(token))
                    return Json(new { success = false, message = "Sesión expirada" });

                var client = _httpClientFactory.CreateClient("SIGEBIAPI");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.PutAsync($"api/Notificaciones/{id}/leida", null);

                if (response.IsSuccessStatusCode)
                    return Json(new { success = true });

                var error = await ApiErrorHelper.GetErrorMessageAsync(response);
                return Json(new { success = false, message = "No se pudo actualizar: " + error });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            try
            {
                var token = HttpContext.Session.GetString("JwtToken");
                if (string.IsNullOrWhiteSpace(token)) return Json(new { count = 0 });

                var usuarioId = HttpContext.Session.GetString("UsuarioId");
                if (string.IsNullOrWhiteSpace(usuarioId)) return Json(new { count = 0 });

                var client = _httpClientFactory.CreateClient("SIGEBIAPI");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync($"api/Notificaciones/usuario/{usuarioId}/count");
                if (response.IsSuccessStatusCode)
                {
                    var countStr = await response.Content.ReadAsStringAsync();
                    return Json(new { count = int.Parse(countStr) });
                }
            }
            catch { }
            return Json(new { count = 0 });
        }
    }
}
