using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SIGEBI.Web.Controllers
{
    public class PrestamoViewModel
    {
        public Guid Id { get; set; }
        public string RecursoTitulo { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaDevolucionEstimada { get; set; }
        public DateTime? FechaDevolucionReal { get; set; }
        public string? EstadoActual { get; set; }
    }

    public class PrestamosIndexViewModel
    {
        public List<PrestamoViewModel> Prestamos { get; set; } = new();
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class PrestamosController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public PrestamosController(IHttpClientFactory httpClientFactory)
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

            var model = new PrestamosIndexViewModel();

            try
            {
                var client = _httpClientFactory.CreateClient("SIGEBIAPI");
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync($"api/Prestamos/usuario/{usuarioId}");

                if (!response.IsSuccessStatusCode)
                {
                    model.ErrorMessage = "No se pudieron cargar los préstamos.";
                    return View(model);
                }

                var json = await response.Content.ReadAsStringAsync();
                model.Prestamos = JsonSerializer.Deserialize<List<PrestamoViewModel>>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? new List<PrestamoViewModel>();
            }
            catch
            {
                model.ErrorMessage = "Error al conectar con el servidor.";
            }

            return View(model);
        }
    }
}
