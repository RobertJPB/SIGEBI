using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SIGEBI.Web.Controllers
{
    public class UsuarioPerfilViewModel
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string? Rol { get; set; }
        public string? Estado { get; set; }
        public string? ImagenUrl { get; set; }
    }

    public class PerfilIndexViewModel
    {
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string? ImagenUrl { get; set; }
    }

    public class PerfilController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public PerfilController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToAction("Login", "Auth");

            var usuarioId = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrWhiteSpace(usuarioId))
                return RedirectToAction("Login", "Auth");

            var model = new PerfilIndexViewModel();

            try
            {
                var client = _httpClientFactory.CreateClient("SIGEBIAPI");
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync($"api/Usuarios/{usuarioId}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var usuario = JsonSerializer.Deserialize<UsuarioPerfilViewModel>(
                        json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (usuario != null)
                    {
                        model.Nombre = usuario.Nombre;
                        model.Correo = usuario.Correo;
                        model.Rol = usuario.Rol ?? "Estudiante";
                        model.Estado = usuario.Estado ?? "Activo";
                        model.ImagenUrl = usuario.ImagenUrl;
                    }
                }
            }
            catch { /* silencioso */ }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarFoto(IFormFile foto)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrWhiteSpace(token)) return RedirectToAction("Login", "Auth");

            if (foto != null && foto.Length > 0)
            {
                try
                {
                    var client = _httpClientFactory.CreateClient("SIGEBIAPI");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    using var content = new MultipartFormDataContent();
                    using var fileStream = foto.OpenReadStream();
                    var fileContent = new StreamContent(fileStream);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(foto.ContentType);
                    content.Add(fileContent, "foto", foto.FileName);

                    var response = await client.PostAsync("api/Usuarios/perfil/foto", content);
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Foto de perfil actualizada correctamente.";
                    }
                    else
                    {
                        TempData["Error"] = "No se pudo actualizar la foto.";
                    }
                }
                catch
                {
                    TempData["Error"] = "Error de comunicación con el servidor.";
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }
    }
}
