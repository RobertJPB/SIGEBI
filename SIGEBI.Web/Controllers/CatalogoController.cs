using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Models;
using System.Text.Json;
using System.Text;
using Refit;
using SIGEBI.Web.Services;
using SIGEBI.Web.Helpers;

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
        private readonly ICatalogoService _catalogoService;
        private readonly IConfiguration _configuration;

        public CatalogoController(ICatalogoService catalogoService, IConfiguration configuration)
        {
            _catalogoService = catalogoService;
            _configuration = configuration;
        }

        private string GetBearerToken()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            return string.IsNullOrWhiteSpace(token) ? string.Empty : $"Bearer {token}";
        }

        public async Task<IActionResult> Index(string? busqueda)
        {
            var model = new CatalogoViewModel
            {
                Busqueda = busqueda ?? string.Empty,
                ApiBaseUrl = _configuration["ApiSettings:BaseUrl"]!
            };

            var token = GetBearerToken();
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            // Consumo mediante Servicio (Capa desacoplada)
            var dtos = await _catalogoService.GetRecursosAsync(token, busqueda);

            model.Recursos = dtos.Select(MapDtoToViewModel).ToList();

            return View(model);
        }

        public async Task<IActionResult> Detalle(Guid id)
        {
            var token = GetBearerToken();
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            // 1. Obtener detalles del recurso mediante servicio (Capa desacoplada)
            var dto = await _catalogoService.GetRecursoAsync(id, token);
            if (dto == null) return RedirectToAction("Index");
            
            var recurso = MapDtoToViewModel(dto);

            // 2. Obtener valoraciones mediante servicio
            try 
            {
                var valoraciones = await _catalogoService.GetValoracionesAsync(id, token);
                recurso.Valoraciones = valoraciones ?? new List<SIGEBI.Business.DTOs.ValoracionDTO>();
            }
            catch (Exception ex)
            { 
                // Mantenemos reporte de error pero sin traza técnica en la UI
                ViewBag.ErrorValoraciones = "El servicio de testimonios no pudo cargar los datos: " + ex.Message;
                recurso.Valoraciones = new List<SIGEBI.Business.DTOs.ValoracionDTO>();
            }

            ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            return View(recurso);
        }

        private RecursoViewModel MapDtoToViewModel(SIGEBI.Business.DTOs.RecursoDetalleDTO dto)
        {
            return new RecursoViewModel
            {
                Id = dto.Id,
                Titulo = dto.Titulo,
                Autor = dto.Autor,
                CategoriaNombre = dto.CategoriaNombre,
                Stock = dto.Stock,
                Estado = dto.Estado,
                TipoRecurso = dto.TipoRecurso,
                ImagenUrl = dto.ImagenUrl,
                Descripcion = dto.Descripcion,
                ISBN = dto.ISBN,
                Editorial = dto.Editorial,
                Anio = dto.Anio,
                Genero = dto.Genero,
                ISSN = dto.ISSN,
                NumeroEdicion = dto.NumeroEdicion,
                Formato = dto.Formato,
                Institucion = dto.Institucion,
                PromedioValoraciones = dto.PromedioValoraciones
            };
        }

        [HttpPost]
        public async Task<IActionResult> SolicitarPrestamo(Guid recursoId)
        {
            try
            {
                var token = GetBearerToken();
                var usuarioIdString = HttpContext.Session.GetString("UsuarioId");

                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(usuarioIdString))
                    return Json(new { success = false, message = "Sesión expirada" });

                await _catalogoService.SolicitarPrestamoAsync(Guid.Parse(usuarioIdString), recursoId, token);

                return Json(new { success = true });
            }
            catch (ApiException apiEx)
            {
                string message = await ApiErrorHelper.GetErrorMessageAsync(apiEx);
                return Json(new { success = false, message = "No se pudo procesar el préstamo: " + message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error interno: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Valorar(Guid recursoId, int calificacion, string comentario)
        {
            try
            {
                var token = GetBearerToken();
                var usuarioIdString = HttpContext.Session.GetString("UsuarioId");

                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(usuarioIdString))
                    return RedirectToAction("Login", "Auth");

                await _catalogoService.ValorarAsync(Guid.Parse(usuarioIdString), recursoId, calificacion, comentario, token);
                return RedirectToAction("Detalle", new { id = recursoId });
            }
            catch (Exception ex)
            {
                // Mostramos el error para saber si es por el Índice Único (ya valoró) u otro motivo
                return Content($"No se pudo registrar su valoración. Detalles: {ex.Message}. Importante: Solo se permite una valoración por recurso.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EliminarValoracion(Guid id, Guid recursoId)
        {
            try
            {
                var token = GetBearerToken();
                if (string.IsNullOrEmpty(token))
                    return Json(new { success = false, message = "Sesión expirada" });

                await _catalogoService.EliminarValoracionAsync(id, token);
                return Json(new { success = true });
            }
            catch (ApiException apiEx)
            {
                var error = await ApiErrorHelper.GetErrorMessageAsync(apiEx);
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
                var token = GetBearerToken();
                var usuarioIdString = HttpContext.Session.GetString("UsuarioId");

                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(usuarioIdString))
                    return Json(new { success = false, message = "Sesión expirada" });

                await _catalogoService.AgregarAListaDeseosAsync(Guid.Parse(usuarioIdString), recursoId, token);
                return Json(new { success = true });
            }
            catch (ApiException apiEx)
            {
                var error = await ApiErrorHelper.GetErrorMessageAsync(apiEx);
                return Json(new { success = false, message = "No se pudo agregar a la lista: " + error });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
