using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SIGEBI.Web.Controllers
{
    public class CatalogoViewModel
    {
        public List<RecursoViewModel> Recursos { get; set; } = new();
        public string Busqueda { get; set; } = string.Empty;
        public string ApiBaseUrl { get; set; } = string.Empty;
    }

    public class CatalogoController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public CatalogoController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(string? busqueda)
        {
            var model = new CatalogoViewModel
            {
                Busqueda = busqueda ?? string.Empty,
                ApiBaseUrl = _configuration["ApiSettings:BaseUrl"]!
            };

            try
            {
                var client = _httpClientFactory.CreateClient("SIGEBIAPI");
                var token = HttpContext.Session.GetString("JwtToken");

                if (string.IsNullOrWhiteSpace(token))
                    return RedirectToAction("Login", "Auth");

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var url = string.IsNullOrWhiteSpace(busqueda)
                    ? "api/Recursos"
                    : $"api/Recursos/buscar?titulo={Uri.EscapeDataString(busqueda)}";

                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return View(model);
                }

                var json = await response.Content.ReadAsStringAsync();
                model.Recursos = JsonSerializer.Deserialize<List<RecursoViewModel>>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? new List<RecursoViewModel>();

                return View(model);
            }
            catch
            {
                return View(model);
            }
        }
    }
}
