using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using SIGEBI.Web.Helpers;

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
        public string ApiBaseUrl { get; set; } = string.Empty;
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

            var apiBaseUrl = _config["ApiSettings:BaseUrl"] ?? "https://localhost:7047/";
            var model = new PerfilIndexViewModel { ApiBaseUrl = apiBaseUrl };

            try
            {
                var client = _httpClientFactory.CreateClient("SIGEBIAPI");
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                // Llamar al endpoint de perfil validado y seguro de la API
                var response = await client.GetAsync("api/Usuarios/perfil");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var usuario = JsonSerializer.Deserialize<SIGEBI.Business.DTOs.UsuarioDTO>(
                        json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (usuario != null)
                    {
                        model.Nombre = usuario.Nombre;
                        model.Correo = usuario.Correo;
                        model.ImagenUrl = usuario.ImagenUrl;

                        // Mapeo manual de Rol (basado en SIGEBI.Domain.Enums.Seguridad.RolUsuario)
                        model.Rol = usuario.IdRol switch
                        {
                            1 => "Administrador",
                            2 => "Bibliotecario",
                            3 => "Estudiante",
                            4 => "Docente",
                            _ => "Usuario"
                        };

                        // Mapeo manual de Estado (basado en SIGEBI.Domain.Enums.Seguridad.EstadoUsuario)
                        model.Estado = usuario.Estado switch
                        {
                            1 => "Activo",
                            2 => "Inactivo",
                            3 => "Suspendido",
                            4 => "Bloqueado",
                            _ => "Desconocido"
                        };
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
        public async Task<IActionResult> ActualizarDatos(string nombre, string correo)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrWhiteSpace(token)) return RedirectToAction("Login", "Auth");

            try
            {
                var client = _httpClientFactory.CreateClient("SIGEBIAPI");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Reutilizamos el DTO de la API o mandamos un objeto anónimo que coincida con ActualizarPerfilRequest
                var requestBody = new { Nombre = nombre, Correo = correo };
                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await client.PutAsync("api/Usuarios/perfil", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Perfil actualizado correctamente.";
                    // Si cambió el nombre, lo actualizamos también en la sesión para que se vea bien en el Layout si se usa allí
                    HttpContext.Session.SetString("NombreUsuario", nombre);
                }
                else
                {
                    var error = await ApiErrorHelper.GetErrorMessageAsync(response);
                    TempData["Error"] = "No se pudo actualizar: " + error;
                }
            }
            catch
            {
                TempData["Error"] = "Error de comunicación con el servidor.";
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
