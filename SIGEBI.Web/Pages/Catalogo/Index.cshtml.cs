using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIGEBI.Web.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SIGEBI.Web.Pages.Catalogo
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public List<RecursoViewModel> Recursos { get; set; } = new();
        public string Busqueda { get; set; } = string.Empty;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> OnGetAsync(string? busqueda)
        {
            Busqueda = busqueda ?? string.Empty;

            try
            {
                var client = _httpClientFactory.CreateClient("SIGEBIAPI");

                var token = HttpContext.Session.GetString("JwtToken");

                if (string.IsNullOrWhiteSpace(token))
                {
                    return RedirectToPage("/Auth/Login");
                }

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var url = string.IsNullOrWhiteSpace(busqueda)
                    ? "api/Recursos"
                    : $"api/Recursos/buscar?titulo={Uri.EscapeDataString(busqueda)}";

                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Recursos = new List<RecursoViewModel>();
                    return Page();
                }

                var json = await response.Content.ReadAsStringAsync();

                Recursos = JsonSerializer.Deserialize<List<RecursoViewModel>>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<RecursoViewModel>();

                return Page();
            }
            catch
            {
                Recursos = new List<RecursoViewModel>();
                return Page();
            }
        }
    }
}