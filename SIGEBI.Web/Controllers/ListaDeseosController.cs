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
        public string? ImagenUrl { get; set; }
    }

    public class ListaDeseosViewModel
    {
        public List<ListaDeseosItemViewModel> Items { get; set; } = new();
        public string Token { get; set; } = string.Empty;
        public string ApiBaseUrl { get; set; } = string.Empty;
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

            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrWhiteSpace(usuarioIdStr))
                return RedirectToAction("Login", "Auth");

            // Pasamos la URL base a la vista para las imagenes
            var model = new ListaDeseosViewModel 
            { 
                Token = token,
                ApiBaseUrl = "https://localhost:7047/" 
            };

            try
            {
                var client = _httpClientFactory.CreateClient("SIGEBIAPI");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync($"api/ListaDeseos/usuario/{usuarioIdStr}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    
                    var apiData = JsonSerializer.Deserialize<ListaDeseosApiResponse>(json, 
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (apiData != null && apiData.Recursos != null)
                    {
                        model.Items = apiData.Recursos.Select(r => new ListaDeseosItemViewModel
                        {
                            Id = r.Id,
                            RecursoTitulo = r.Titulo,
                            RecursoAutor = r.Autor,
                            CategoriaNombre = r.CategoriaNombre,
                            ImagenUrl = r.ImagenUrl
                        }).ToList();
                    }
                }
            }
            catch { /* Items queda vacío si hay error */ }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Quitar(Guid id)
        {
            try
            {
                var token = HttpContext.Session.GetString("JwtToken");
                var usuarioId = HttpContext.Session.GetString("UsuarioId");

                if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(usuarioId))
                    return Json(new { success = false, message = "Sesión expirada" });

                var client = _httpClientFactory.CreateClient("SIGEBIAPI");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // El API usa DELETE api/ListaDeseos/usuario/{u}/recurso/{r}
                var response = await client.DeleteAsync($"api/ListaDeseos/usuario/{usuarioId}/recurso/{id}");

                if (response.IsSuccessStatusCode)
                    return Json(new { success = true });

                var error = await response.Content.ReadAsStringAsync();
                return Json(new { success = false, message = "No se pudo quitar: " + error });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Clases auxiliares para mapear la respuesta del API (ListaDeseosDTO)
        private class ListaDeseosApiResponse
        {
            public List<RecursoApiItem> Recursos { get; set; } = new();
        }

        private class RecursoApiItem
        {
            public Guid Id { get; set; }
            public string Titulo { get; set; } = string.Empty;
            public string Autor { get; set; } = string.Empty;
            public string CategoriaNombre { get; set; } = string.Empty;
            public string? ImagenUrl { get; set; }
        }
    }
}
