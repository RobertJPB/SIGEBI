using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApiDTOs = SIGEBI.API.DTOs;
using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces.UseCases.Catalogo;
using SIGEBI.Business.Interfaces.Services;
using SIGEBI.Business.Validators;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Domain.Enums.Seguridad;
using SIGEBI.API.Extensions;

namespace SIGEBI.API.Controllers
{
    /// <summary>
    /// Controlador para la gestión de recursos bibliográficos (Libros, Revistas, Documentos).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RecursosController : ControllerBase
    {
        private readonly IConsultarLibrosUseCase _consultarUseCase;
        private readonly IGestionarRecursosUseCase _gestionarUseCase;
        private readonly IImagenService _imagenService;
        private readonly AgregarRecursoValidator _validator;

        public RecursosController(
            IConsultarLibrosUseCase consultarUseCase,
            IGestionarRecursosUseCase gestionarUseCase,
            IImagenService imagenService,
            AgregarRecursoValidator validator)
        {
            _consultarUseCase = consultarUseCase;
            _gestionarUseCase = gestionarUseCase;
            _imagenService = imagenService;
            _validator = validator;
        }

        // ── GET ──
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecursoDetalleDTO>>> ObtenerTodos()
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "ver catálogo");

            var recursos = await _consultarUseCase.EjecutarAsync();
            return Ok(recursos);
        }

        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<RecursoDetalleDTO>>> BuscarPorTitulo([FromQuery] string titulo)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "buscar recursos");

            var recursos = await _consultarUseCase.BuscarPorTituloAsync(titulo);
            return Ok(recursos);
        }

        [HttpGet("categoria/{categoriaId:int}")]
        public async Task<ActionResult<IEnumerable<RecursoDetalleDTO>>> ObtenerPorCategoria(int categoriaId)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "ver por categoría");

            var recursos = await _consultarUseCase.BuscarPorCategoriaAsync(categoriaId);
            return Ok(recursos);
        }

        [HttpGet("{id:guid}", Name = "ObtenerRecursoPorId")]
        public async Task<ActionResult<RecursoDetalleDTO>> ObtenerPorId(Guid id)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "ver detalle de recurso");

            var recurso = await _consultarUseCase.GetByIdAsync(id);
            if (recurso == null) return NotFound(new { mensaje = "Recurso no encontrado." });

            return Ok(recurso);
        }

        // ── POST ──
        [HttpPost("libro")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AgregarLibro([FromForm] ApiDTOs.AgregarLibroRequest request)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarRecursos(rol), "agregar libro");

            // Validación de negocio simplificada
            var validacionDto = new RecursoDetalleDTO { Titulo = request.Titulo, Autor = request.Autor, Stock = request.Stock, TipoRecurso = "Libro", ISBN = request.ISBN };
            var errores = _validator.Validar(validacionDto);
            if (errores.Any()) return BadRequest(new { errores });
            
            // Guardado usando Streams
            string? imagenUrl = null;
            if (request.Imagen != null)
            {
                using var stream = request.Imagen.OpenReadStream();
                imagenUrl = await _imagenService.GuardarImagenAsync(stream, request.Imagen.FileName, "recursos");
            }

            var resultado = await _gestionarUseCase.AgregarLibroAsync(
                request.Titulo, request.Autor, request.CategoriaId, request.Stock, request.Descripcion,
                request.ISBN, request.Editorial, request.Anio, imagenUrl, request.Genero);

            return CreatedAtAction(nameof(ObtenerPorId), new { id = resultado.Id }, resultado);
        }

        [HttpPost("revista")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AgregarRevista([FromForm] ApiDTOs.AgregarRevistaRequest request)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarRecursos(rol), "agregar revista");

            string? imagenUrl = null;
            if (request.Imagen != null)
            {
                using var stream = request.Imagen.OpenReadStream();
                imagenUrl = await _imagenService.GuardarImagenAsync(stream, request.Imagen.FileName, "recursos");
            }

            var resultado = await _gestionarUseCase.AgregarRevistaAsync(
                request.Titulo, request.Autor, request.CategoriaId, request.Stock, request.Descripcion,
                request.NumeroEdicion, request.ISSN, request.Anio, request.Editorial, imagenUrl);

            return CreatedAtAction(nameof(ObtenerPorId), new { id = resultado.Id }, resultado);
        }

        [HttpPost("documento")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AgregarDocumento([FromForm] ApiDTOs.AgregarDocumentoRequest request)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarRecursos(rol), "agregar documento");

            string? imagenUrl = null;
            if (request.Imagen != null)
            {
                using var stream = request.Imagen.OpenReadStream();
                imagenUrl = await _imagenService.GuardarImagenAsync(stream, request.Imagen.FileName, "recursos");
            }

            var resultado = await _gestionarUseCase.AgregarDocumentoAsync(
                request.Titulo, request.Autor, request.CategoriaId, request.Stock, request.Descripcion,
                request.Formato, request.Institucion, request.Anio, imagenUrl);

            return CreatedAtAction(nameof(ObtenerPorId), new { id = resultado.Id }, resultado);
        }

        // ── PUT ──
        [HttpPut("libro/{id:guid}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> EditarLibro(Guid id, [FromForm] ApiDTOs.AgregarLibroRequest request)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarRecursos(rol), "editar libro");

            string? imagenUrl = null;
            if (request.Imagen != null)
            {
                using var stream = request.Imagen.OpenReadStream();
                imagenUrl = await _imagenService.GuardarImagenAsync(stream, request.Imagen.FileName, "recursos");
            }

            var resultado = await _gestionarUseCase.EditarLibroAsync(
                id, request.Titulo, request.Autor, request.CategoriaId, request.Stock, request.Descripcion,
                request.ISBN, request.Editorial, request.Anio, imagenUrl, request.Genero);

            return Ok(resultado);
        }

        [HttpPut("revista/{id:guid}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> EditarRevista(Guid id, [FromForm] ApiDTOs.AgregarRevistaRequest request)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarRecursos(rol), "editar revista");

            string? imagenUrl = null;
            if (request.Imagen != null)
            {
                using var stream = request.Imagen.OpenReadStream();
                imagenUrl = await _imagenService.GuardarImagenAsync(stream, request.Imagen.FileName, "recursos");
            }
            var resultado = await _gestionarUseCase.EditarRevistaAsync(
                id, request.Titulo, request.Autor, request.CategoriaId, request.Stock, request.Descripcion,
                request.NumeroEdicion, request.ISSN, request.Anio, request.Editorial, imagenUrl);

            return Ok(resultado);
        }

        [HttpPut("documento/{id:guid}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> EditarDocumento(Guid id, [FromForm] ApiDTOs.AgregarDocumentoRequest request)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarRecursos(rol), "editar documento");

            string? imagenUrl = null;
            if (request.Imagen != null)
            {
                using var stream = request.Imagen.OpenReadStream();
                imagenUrl = await _imagenService.GuardarImagenAsync(stream, request.Imagen.FileName, "recursos");
            }
            var resultado = await _gestionarUseCase.EditarDocumentoAsync(
                id, request.Titulo, request.Autor, request.CategoriaId, request.Stock, request.Descripcion,
                request.Formato, request.Institucion, request.Anio, imagenUrl);

            return Ok(resultado);
        }

        // ── DELETE ──
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Eliminar(Guid id)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarRecursos(rol), "eliminar recurso");

            // Opcional: Podríamos borrar la imagen física aquí inyectando IImagenService.
            // Para mantener la consistencia, el UseCase debería informarnos la URL a borrar.
            var recurso = await _consultarUseCase.GetByIdAsync(id);
            if (recurso != null) _imagenService.EliminarImagen(recurso.ImagenUrl);

            await _gestionarUseCase.EliminarRecursoAsync(id);
            return NoContent(); // 204 es más adecuado para eliminaciones exitosas sin contenido
        }
    }
}
