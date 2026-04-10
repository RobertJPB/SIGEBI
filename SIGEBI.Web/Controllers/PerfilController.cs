using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Services;
using SIGEBI.Web.Helpers;
using Refit;

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
        private readonly IUsuarioService _usuarioService;
        private readonly IConfiguration _config;

        public PerfilController(IUsuarioService usuarioService, IConfiguration config)
        {
            _usuarioService = usuarioService;
            _config = config;
        }

        private string GetBearerToken()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            return string.IsNullOrWhiteSpace(token) ? string.Empty : $"Bearer {token}";
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var token = GetBearerToken();
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            var apiBaseUrl = _config["ApiSettings:BaseUrl"] ?? "https://localhost:7047/";
            var model = new PerfilIndexViewModel { ApiBaseUrl = apiBaseUrl };

            try
            {
                var usuario = await _usuarioService.GetPerfilAsync(token);

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
            catch { /* silencioso */ }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarFoto(IFormFile foto)
        {
            var token = GetBearerToken();
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            if (foto != null && foto.Length > 0)
            {
                try
                {
                    await _usuarioService.ActualizarFotoAsync(foto, token);
                    TempData["Success"] = "Foto de perfil actualizada correctamente.";
                }
                catch (ApiException apiEx)
                {
                     var error = await ApiErrorHelper.GetErrorMessageAsync(apiEx);
                     TempData["Error"] = "No se pudo actualizar la foto: " + error;
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error inesperado: " + ex.Message;
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarDatos(string nombre, string correo)
        {
            var token = GetBearerToken();
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            try
            {
                await _usuarioService.ActualizarDatosAsync(nombre, correo, token);
                
                TempData["Success"] = "Perfil actualizado correctamente.";
                HttpContext.Session.SetString("UsuarioNombre", nombre);
            }
            catch (ApiException apiEx)
            {
                var error = await ApiErrorHelper.GetErrorMessageAsync(apiEx);
                TempData["Error"] = "No se pudo actualizar: " + error;
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error inesperado: " + ex.Message;
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
