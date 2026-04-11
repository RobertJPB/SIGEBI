using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using Refit;
using SIGEBI.Web.Services;
using SIGEBI.Web.Helpers;

namespace SIGEBI.Web.Controllers
{
    public class LoginViewModel
    {
        public string Correo { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Correo) || string.IsNullOrWhiteSpace(model.Contrasena))
            {
                model.ErrorMessage = "Correo y contraseña son obligatorios.";
                return View(model);
            }

            try
            {
                // Consumo mediante Servicio (Capa desacoplada)
                var result = await _authService.LoginAsync(model.Correo, model.Contrasena);

                if (!result.TryGetProperty("token", out var tokenElement))
                {
                    model.ErrorMessage = "La API no devolvió token.";
                    return View(model);
                }

                var token = tokenElement.GetString();

                if (string.IsNullOrWhiteSpace(token))
                {
                    model.ErrorMessage = "El token llegó vacío.";
                    return View(model);
                }

                // Guardar token en Session
                HttpContext.Session.SetString("JwtToken", token);

                //  Extraer el UsuarioId del token JWT y guardarlo en sesión
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var usuarioIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                
                if (!string.IsNullOrEmpty(usuarioIdClaim))
                {
                    HttpContext.Session.SetString("UsuarioId", usuarioIdClaim);
                }

                var usuarioRolClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (!string.IsNullOrEmpty(usuarioRolClaim))
                {
                    HttpContext.Session.SetString("UsuarioRol", usuarioRolClaim);
                }

                var usuarioNombreClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                if (!string.IsNullOrEmpty(usuarioNombreClaim))
                {
                    HttpContext.Session.SetString("UsuarioNombre", usuarioNombreClaim);
                }

                var usuarioImagenClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "ImagenUrl")?.Value;
                if (!string.IsNullOrEmpty(usuarioImagenClaim))
                {
                    HttpContext.Session.SetString("UsuarioImagen", usuarioImagenClaim);
                }

                // Crear cookie de autenticación
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, usuarioIdClaim ?? ""),
                    new Claim(ClaimTypes.Role, usuarioRolClaim ?? ""),
                    new Claim("JwtToken", token),
                    new Claim("UsuarioImagen", usuarioImagenClaim ?? "")
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties
                );

                return RedirectToAction("Index", "Catalogo");
            }
            catch (ApiException apiEx)
            {
                var message = await ApiErrorHelper.GetErrorMessageAsync(apiEx);
                model.ErrorMessage = message;
                return View(model);
            }
            catch (Exception ex)
            {
                // Si es un error de red/conexión, dejamos que el filtro global redirija a Mantenimiento
                if (ApiErrorHelper.EsErrorDeConexion(ex)) throw;

                model.ErrorMessage = $"Error inesperado: {ex.Message}";
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string? message)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();

            if (!string.IsNullOrEmpty(message))
            {
                TempData["LogoutMessage"] = message;
            }

            return RedirectToAction("Login");
        }
    }
}
