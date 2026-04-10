using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using Refit;
using SIGEBI.Web.Services;
using SIGEBI.Web.Helpers;

namespace SIGEBI.Web.Controllers
{
    public class PrestamoViewModel
    {
        public Guid Id { get; set; }
        public string TituloRecurso { get; set; } = string.Empty;
        public string FechaInicio { get; set; } = string.Empty;
        public string FechaDevolucionEstimada { get; set; } = string.Empty;
        public string? FechaDevolucionReal { get; set; }
        public string Estado { get; set; } = string.Empty;
    }

    public class PrestamosIndexViewModel
    {
        public List<PrestamoViewModel> Prestamos { get; set; } = new();
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class PrestamosController : Controller
    {
        private readonly IPrestamoService _prestamoService;

        public PrestamosController(IPrestamoService prestamoService)
        {
            _prestamoService = prestamoService;
        }

        private string GetBearerToken()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            return string.IsNullOrWhiteSpace(token) ? string.Empty : $"Bearer {token}";
        }

        public async Task<IActionResult> Index()
        {
            var token = GetBearerToken();
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr) || !Guid.TryParse(usuarioIdStr, out var usuarioId))
                return RedirectToAction("Login", "Auth");

            var model = new PrestamosIndexViewModel();

            try
            {
                // Consumo mediante Servicio Desacoplado (Punto 2 Actividades)
                var dtos = await _prestamoService.GetPrestamosByUsuarioAsync(usuarioId, token);
                
                model.Prestamos = dtos.Select(d => new PrestamoViewModel
                {
                    Id = d.Id,
                    TituloRecurso = d.TituloRecurso,
                    FechaInicio = d.FechaInicio,
                    FechaDevolucionEstimada = d.FechaDevolucionEstimada,
                    FechaDevolucionReal = d.FechaDevolucionReal,
                    Estado = d.Estado
                }).ToList();
            }
            catch (ApiException apiEx)
            {
                var error = await ApiErrorHelper.GetErrorMessageAsync(apiEx);
                model.ErrorMessage = $"Error al obtener datos de la API: {error}";
            }
            catch (Exception ex)
            {
                model.ErrorMessage = $"Error inesperado: {ex.Message}";
            }

            return View(model);
        }
    }
}
