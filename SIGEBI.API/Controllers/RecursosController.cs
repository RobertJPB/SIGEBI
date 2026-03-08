using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.DTOs;
using SIGEBI.Business.UseCases.Catalogo;
using SIGEBI.Business.Validators;

namespace SIGEBI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecursosController : ControllerBase
    {
        private readonly ConsultarLibrosUseCase _consultarLibros;
        private readonly GestionarRecursosUseCase _gestionarRecursos;
        private readonly AgregarRecursoValidator _validator;

        public RecursosController(
            ConsultarLibrosUseCase consultarLibros,
            GestionarRecursosUseCase gestionarRecursos,
            AgregarRecursoValidator validator)
        {
            _consultarLibros = consultarLibros;
            _gestionarRecursos = gestionarRecursos;
            _validator = validator;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var recursos = await _consultarLibros.EjecutarAsync();
            return Ok(recursos);
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> BuscarPorTitulo([FromQuery] string titulo)
        {
            var recursos = await _consultarLibros.BuscarPorTituloAsync(titulo);
            return Ok(recursos);
        }

        [HttpGet("categoria/{categoriaId}")]
        public async Task<IActionResult> BuscarPorCategoria(int categoriaId)
        {
            var recursos = await _consultarLibros.BuscarPorCategoriaAsync(categoriaId);
            return Ok(recursos);
        }

        [HttpPost("libro")]
        public async Task<IActionResult> AgregarLibro(RecursoDetalleDTO dto)
        {
            var errores = _validator.Validar(dto);
            if (errores.Any())
                return BadRequest(errores);

            var resultado = await _gestionarRecursos.AgregarLibroAsync(
                dto.Titulo, dto.Autor, dto.CategoriaId, dto.Stock,
                dto.ISBN!, dto.Editorial!, dto.Anio!.Value);
            return Ok(resultado);
        }

        [HttpPost("revista")]
        public async Task<IActionResult> AgregarRevista(RecursoDetalleDTO dto)
        {
            var errores = _validator.Validar(dto);
            if (errores.Any())
                return BadRequest(errores);

            var resultado = await _gestionarRecursos.AgregarRevistaAsync(
                dto.Titulo, dto.Autor, dto.CategoriaId, dto.Stock,
                dto.NumeroEdicion!.Value, dto.ISSN!, dto.FechaPublicacion!.Value);
            return Ok(resultado);
        }

        [HttpPost("documento")]
        public async Task<IActionResult> AgregarDocumento(RecursoDetalleDTO dto)
        {
            var errores = _validator.Validar(dto);
            if (errores.Any())
                return BadRequest(errores);

            var resultado = await _gestionarRecursos.AgregarDocumentoAsync(
                dto.Titulo, dto.Autor, dto.CategoriaId, dto.Stock,
                dto.Formato!, dto.Institucion!, dto.Anio!.Value);
            return Ok(resultado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(Guid id)
        {
            await _gestionarRecursos.EliminarRecursoAsync(id);
            return NoContent();
        }
    }
}