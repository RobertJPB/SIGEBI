using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using Refit;
using SIGEBI.Web.Services;

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
        private readonly ISigebiApi _api;

        public AuthController(ISigebiApi api)
        {
            _api = api;
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
                var payload = new { correo = model.Correo, contrasena = model.Contrasena };
                
                // Consumo mediante Servicio (Punto 2 Actividades)
                var result = await _api.LoginAsync(payload);

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

                // Crear cookie de autenticación
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuarioNombreClaim ?? model.Correo),
                    new Claim(ClaimTypes.NameIdentifier, usuarioIdClaim ?? ""),
                    new Claim(ClaimTypes.Role, usuarioRolClaim ?? ""),
                    new Claim("JwtToken", token)
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
            catch (Exception ex)
            {
                model.ErrorMessage = $"No se pudo conectar con el servidor. Detalle: {ex.Message}";
                return View(model);
            }
        }
    }
}
