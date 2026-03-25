using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
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

        public async Task<IActionResult> Detalle(Guid id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("SIGEBIAPI");
                var token = HttpContext.Session.GetString("JwtToken");

                if (string.IsNullOrWhiteSpace(token))
                    return RedirectToAction("Login", "Auth");

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                // 1. Obtener detalles del recurso
                var response = await client.GetAsync($"api/Recursos/{id}");
                if (!response.IsSuccessStatusCode)
                    return RedirectToAction("Index");

                var json = await response.Content.ReadAsStringAsync();
                var recurso = JsonSerializer.Deserialize<RecursoViewModel>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (recurso == null)
                    return RedirectToAction("Index");

                // 2. Obtener valoraciones/comentarios
                var valResponse = await client.GetAsync($"api/Valoraciones/recurso/{id}");
                if (valResponse.IsSuccessStatusCode)
                {
                    var valJson = await valResponse.Content.ReadAsStringAsync();
                    recurso.Valoraciones = JsonSerializer.Deserialize<List<SIGEBI.Business.DTOs.ValoracionDTO>>(
                        valJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                }

                ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
                return View(recurso);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SolicitarPrestamo(Guid recursoId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("SIGEBIAPI");
                var token = HttpContext.Session.GetString("JwtToken");
                var usuarioId = HttpContext.Session.GetString("UsuarioId");

                if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(usuarioId))
                    return Json(new { success = false, message = "Sesión expirada" });

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var request = new { UsuarioId = Guid.Parse(usuarioId), RecursoId = recursoId };
                var response = await client.PostAsJsonAsync("api/Prestamos", request);

                if (response.IsSuccessStatusCode)
                    return Json(new { success = true });
                
                var error = await response.Content.ReadAsStringAsync();
                return Json(new { success = false, message = "No se pudo procesar el préstamo: " + error });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Valorar(Guid recursoId, int calificacion, string comentario)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("SIGEBIAPI");
                var token = HttpContext.Session.GetString("JwtToken");
                var usuarioId = HttpContext.Session.GetString("UsuarioId");

                if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(usuarioId))
                    return RedirectToAction("Login", "Auth");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var request = new
                {
                    UsuarioId = Guid.Parse(usuarioId),
                    RecursoId = recursoId,
                    Calificacion = calificacion,
                    Comentario = comentario
                };

                await client.PostAsJsonAsync("api/Valoraciones", request);
                return RedirectToAction("Detalle", new { id = recursoId });
            }
            catch
            {
                return RedirectToAction("Detalle", new { id = recursoId });
            }
        }

        [HttpPost]
        public async Task<IActionResult> EliminarValoracion(Guid id, Guid recursoId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("SIGEBIAPI");
                var token = HttpContext.Session.GetString("JwtToken");

                if (string.IsNullOrWhiteSpace(token))
                    return Json(new { success = false, message = "Sesión expirada" });

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.DeleteAsync($"api/Valoraciones/{id}");

                if (response.IsSuccessStatusCode)
                    return Json(new { success = true });

                var error = await response.Content.ReadAsStringAsync();
                return Json(new { success = false, message = "No se pudo eliminar: " + error });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AgregarAListaDeseos(Guid recursoId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("SIGEBIAPI");
                var token = HttpContext.Session.GetString("JwtToken");
                var usuarioId = HttpContext.Session.GetString("UsuarioId");

                if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(usuarioId))
                    return Json(new { success = false, message = "Sesión expirada" });

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.PostAsync($"api/ListaDeseos/usuario/{usuarioId}/recurso/{recursoId}", null);

                if (response.IsSuccessStatusCode)
                    return Json(new { success = true });

                var error = await response.Content.ReadAsStringAsync();
                return Json(new { success = false, message = "No se pudo agregar a la lista: " + error });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
