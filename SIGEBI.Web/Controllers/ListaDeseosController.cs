using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SIGEBI.Web.Controllers
{
    public class ListaDeseosItemViewModel
    {
        public Guid Id { get; set; }
        public string RecursoTitulo { get; set; } = string.Empty;
        public string RecursoAutor { get; set; } = string.Empty;
        public string CategoriaNombre { get; set; } = string.Empty;
    }

    public class ListaDeseosViewModel
    {
        public List<ListaDeseosItemViewModel> Items { get; set; } = new();
        public string Token { get; set; } = string.Empty;
    }

    public class ListaDeseosController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ListaDeseosController(IHttpClientFactory httpClientFactory)
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

            var model = new ListaDeseosViewModel
            {
                Token = token
            };

            try
            {
                var client = _httpClientFactory.CreateClient("SIGEBIAPI");
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync($"api/ListaDeseos/usuario/{usuarioId}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    model.Items = JsonSerializer.Deserialize<List<ListaDeseosItemViewModel>>(
                        json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                        ?? new List<ListaDeseosItemViewModel>();
                }
            }
            catch { /* silencioso, Items queda vacío */ }

            return View(model);
        }
    }
}
