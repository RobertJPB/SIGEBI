using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Models;
using System.Text.Json;
using System.Text;
using Refit;
using SIGEBI.Web.Services;

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
        private readonly ISigebiApi _api;
        private readonly IConfiguration _configuration;

        public CatalogoController(ISigebiApi api, IConfiguration configuration)
        {
            _api = api;
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

            try
            {
                var token = GetBearerToken();
                if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

                // Consumo mediante Servicio 
                var dtos = string.IsNullOrWhiteSpace(busqueda)
                    ? await _api.GetRecursosAsync(token)
                    : await _api.BuscarRecursosAsync(busqueda, token);

                model.Recursos = dtos.Select(MapDtoToViewModel).ToList();

                return View(model);
            }
            catch (Exception)
            {
                // En producción registraríamos el error
                return View(model);
            }
        }

        public async Task<IActionResult> Detalle(Guid id)
        {
            try
            {
                var token = GetBearerToken();
                if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

                // 1. Obtener detalles del recurso mediante servicio
                var dto = await _api.GetRecursoAsync(id, token);
                var recurso = MapDtoToViewModel(dto);

                // 2. Obtener valoraciones mediante servicio
                try 
                {
                    recurso.Valoraciones = await _api.GetValoracionesAsync(id, token);
                }
                catch { /* Ignorar si no hay valoraciones */ }

                ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
                return View(recurso);
            }
            catch
            {
                return RedirectToAction("Index");
            }
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
                var usuarioId = HttpContext.Session.GetString("UsuarioId");

                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(usuarioId))
                    return Json(new { success = false, message = "Sesión expirada" });

                var request = new { UsuarioId = Guid.Parse(usuarioId), RecursoId = recursoId };
                await _api.SolicitarPrestamoAsync(request, token);

                return Json(new { success = true });
            }
            catch (ApiException apiEx)
            {
                var error = await apiEx.GetContentAsAsync<string>();
                return Json(new { success = false, message = "No se pudo procesar el préstamo: " + (error ?? apiEx.ReasonPhrase) });
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
                var token = GetBearerToken();
                var usuarioId = HttpContext.Session.GetString("UsuarioId");

                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(usuarioId))
                    return RedirectToAction("Login", "Auth");

                var request = new
                {
                    UsuarioId = Guid.Parse(usuarioId),
                    RecursoId = recursoId,
                    Calificacion = calificacion,
                    Comentario = comentario
                };

                await _api.ValorarAsync(request, token);
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
                var token = GetBearerToken();
                if (string.IsNullOrEmpty(token))
                    return Json(new { success = false, message = "Sesión expirada" });

                await _api.EliminarValoracionAsync(id, token);
                return Json(new { success = true });
            }
            catch (ApiException apiEx)
            {
                var error = await apiEx.GetContentAsAsync<string>();
                return Json(new { success = false, message = "No se pudo eliminar: " + (error ?? apiEx.ReasonPhrase) });
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
                var usuarioId = HttpContext.Session.GetString("UsuarioId");

                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(usuarioId))
                    return Json(new { success = false, message = "Sesión expirada" });

                await _api.AgregarAListaDeseosAsync(Guid.Parse(usuarioId), recursoId, token);
                return Json(new { success = true });
            }
            catch (ApiException apiEx)
            {
                var error = await apiEx.GetContentAsAsync<string>();
                return Json(new { success = false, message = "No se pudo agregar a la lista: " + (error ?? apiEx.ReasonPhrase) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
