using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces.UseCases.Catalogo;
using SIGEBI.Domain.DomainServices;
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

        public RecursosController(
            IConsultarLibrosUseCase consultarUseCase,
            IGestionarRecursosUseCase gestionarUseCase)
        {
            _consultarUseCase = consultarUseCase;
            _gestionarUseCase = gestionarUseCase;
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

        [HttpGet("autores")]
        public async Task<ActionResult<IEnumerable<string>>> ObtenerAutores()
        {
            var autores = await _gestionarUseCase.ObtenerAutoresAsync();
            return Ok(autores);
        }

        [HttpGet("editoriales")]
        public async Task<ActionResult<IEnumerable<string>>> ObtenerEditoriales()
        {
            var editoriales = await _gestionarUseCase.ObtenerEditorialesAsync();
            return Ok(editoriales);
        }

        // ── POST ──
        [HttpPost("libro")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AgregarLibro([FromForm] LibroRequestDTO request, IFormFile? imagen)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarRecursos(rol), "agregar libro");

            if (imagen != null)
            {
                request.ImagenStream = imagen.OpenReadStream();
                request.ImagenNombre = imagen.FileName;
            }

            var resultado = await _gestionarUseCase.AgregarLibroAsync(request, User.ObtenerIdActual());
            return CreatedAtAction(nameof(ObtenerPorId), new { id = resultado.Id }, resultado);
        }

        [HttpPost("revista")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AgregarRevista([FromForm] RevistaRequestDTO request, IFormFile? imagen)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarRecursos(rol), "agregar revista");

            if (imagen != null)
            {
                request.ImagenStream = imagen.OpenReadStream();
                request.ImagenNombre = imagen.FileName;
            }

            var resultado = await _gestionarUseCase.AgregarRevistaAsync(request, User.ObtenerIdActual());
            return CreatedAtAction(nameof(ObtenerPorId), new { id = resultado.Id }, resultado);
        }

        [HttpPost("documento")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AgregarDocumento([FromForm] DocumentoRequestDTO request, IFormFile? imagen)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarRecursos(rol), "agregar documento");

            if (imagen != null)
            {
                request.ImagenStream = imagen.OpenReadStream();
                request.ImagenNombre = imagen.FileName;
            }

            var resultado = await _gestionarUseCase.AgregarDocumentoAsync(request, User.ObtenerIdActual());
            return CreatedAtAction(nameof(ObtenerPorId), new { id = resultado.Id }, resultado);
        }

        // ── PUT ──
        [HttpPut("libro/{id:guid}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> EditarLibro(Guid id, [FromForm] LibroRequestDTO request, IFormFile? imagen)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarRecursos(rol), "editar libro");

            if (imagen != null)
            {
                request.ImagenStream = imagen.OpenReadStream();
                request.ImagenNombre = imagen.FileName;
            }

            await _gestionarUseCase.EditarLibroAsync(id, request);
            return NoContent();
        }

        [HttpPut("revista/{id:guid}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> EditarRevista(Guid id, [FromForm] RevistaRequestDTO request, IFormFile? imagen)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarRecursos(rol), "editar revista");

            if (imagen != null)
            {
                request.ImagenStream = imagen.OpenReadStream();
                request.ImagenNombre = imagen.FileName;
            }

            await _gestionarUseCase.EditarRevistaAsync(id, request);
            return NoContent();
        }

        [HttpPut("documento/{id:guid}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> EditarDocumento(Guid id, [FromForm] DocumentoRequestDTO request, IFormFile? imagen)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarRecursos(rol), "editar documento");

            if (imagen != null)
            {
                request.ImagenStream = imagen.OpenReadStream();
                request.ImagenNombre = imagen.FileName;
            }

            await _gestionarUseCase.EditarDocumentoAsync(id, request);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Eliminar(Guid id)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarRecursos(rol), "eliminar recurso");

            await _gestionarUseCase.EliminarRecursoAsync(id);
            return NoContent();
        }
    }
}
