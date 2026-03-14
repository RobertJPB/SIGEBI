using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace SIGEBI.Web.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        [BindProperty]
        public string Correo { get; set; } = string.Empty;

        [BindProperty]
        public string Contrasena { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        public LoginModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Correo) || string.IsNullOrWhiteSpace(Contrasena))
            {
                ErrorMessage = "Correo y contraseña son obligatorios.";
                return Page();
            }

            try
            {
                var client = _httpClientFactory.CreateClient("SIGEBIAPI");

                var payload = new
                {
                    correo = Correo,
                    contrasena = Contrasena
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/Auth/login", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorMessage = $"Credenciales incorrectas. Código: {(int)response.StatusCode}";
                    return Page();
                }

                var result = JsonSerializer.Deserialize<JsonElement>(responseBody);

                if (!result.TryGetProperty("token", out var tokenElement))
                {
                    ErrorMessage = $"La API no devolvió token. Respuesta: {responseBody}";
                    return Page();
                }

                var token = tokenElement.GetString();

                if (string.IsNullOrWhiteSpace(token))
                {
                    ErrorMessage = "El token llegó vacío.";
                    return Page();
                }

                // Guardar token en Session
                HttpContext.Session.SetString("JwtToken", token);

                // Crear cookie de autenticación
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, Correo),
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

                return RedirectToPage("/Catalogo/Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"No se pudo conectar con el servidor. Detalle: {ex.Message}";
                return Page();
            }
        }
    }
}