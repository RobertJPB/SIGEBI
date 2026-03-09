using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.DTOs;
using SIGEBI.Business.UseCases.Catalogo;

namespace SIGEBI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RecursosController : ControllerBase
    {
        private readonly ConsultarLibrosUseCase _consultarUseCase;
        private readonly GestionarRecursosUseCase _gestionarUseCase;

        public RecursosController(
            ConsultarLibrosUseCase consultarUseCase,
            GestionarRecursosUseCase gestionarUseCase)
        {
            _consultarUseCase = consultarUseCase;
            _gestionarUseCase = gestionarUseCase;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var recursos = await _consultarUseCase.EjecutarAsync();
            return Ok(recursos);
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> BuscarPorTitulo([FromQuery] string titulo)
        {
            var recursos = await _consultarUseCase.BuscarPorTituloAsync(titulo);
            return Ok(recursos);
        }

        [HttpGet("categoria/{categoriaId}")]
        public async Task<IActionResult> GetPorCategoria(int categoriaId)
        {
            var recursos = await _consultarUseCase.BuscarPorCategoriaAsync(categoriaId);
            return Ok(recursos);
        }

        [HttpPost("libro")]
        public async Task<IActionResult> AgregarLibro([FromBody] AgregarLibroRequest request)
        {
            if (request == null) return BadRequest("Datos inválidos.");
            var resultado = await _gestionarUseCase.AgregarLibroAsync(
                request.Titulo, request.Autor, request.CategoriaId, request.Stock,
                request.ISBN, request.Editorial, request.Anio);
            return Ok(resultado);
        }

        [HttpPost("revista")]
        public async Task<IActionResult> AgregarRevista([FromBody] AgregarRevistaRequest request)
        {
            if (request == null) return BadRequest("Datos inválidos.");
            var resultado = await _gestionarUseCase.AgregarRevistaAsync(
                request.Titulo, request.Autor, request.CategoriaId, request.Stock,
                request.NumeroEdicion, request.ISSN, request.FechaPublicacion);
            return Ok(resultado);
        }

        [HttpPost("documento")]
        public async Task<IActionResult> AgregarDocumento([FromBody] AgregarDocumentoRequest request)
        {
            if (request == null) return BadRequest("Datos inválidos.");
            var resultado = await _gestionarUseCase.AgregarDocumentoAsync(
                request.Titulo, request.Autor, request.CategoriaId, request.Stock,
                request.Formato, request.Institucion, request.Anio);
            return Ok(resultado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(Guid id)
        {
            await _gestionarUseCase.EliminarRecursoAsync(id);
            return Ok("Recurso eliminado correctamente.");
        }
    }

    public class AgregarLibroRequest
    {
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public int Stock { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public string Editorial { get; set; } = string.Empty;
        public int Anio { get; set; }
    }

    public class AgregarRevistaRequest
    {
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public int Stock { get; set; }
        public int NumeroEdicion { get; set; }
        public string ISSN { get; set; } = string.Empty;
        public DateTime FechaPublicacion { get; set; }
    }

    public class AgregarDocumentoRequest
    {
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public int Stock { get; set; }
        public string Formato { get; set; } = string.Empty;
        public string Institucion { get; set; } = string.Empty;
        public int Anio { get; set; }
    }
}