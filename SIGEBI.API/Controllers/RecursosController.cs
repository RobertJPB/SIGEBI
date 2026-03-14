using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.DTOs;
using SIGEBI.Business.UseCases.Catalogo;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Domain.Enums.Seguridad;

namespace SIGEBI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    // Principio SOLID (SRP - Responsabilidad Única):
    // Maneja la entrada y salida de datos del catalogo. No sabe nada de SQL ni de validaciones complejas.
    public class RecursosController : ControllerBase
    {
        private readonly ConsultarLibrosUseCase _consultarUseCase;
        private readonly GestionarRecursosUseCase _gestionarUseCase;
        private readonly IWebHostEnvironment _env;

        public RecursosController(
            ConsultarLibrosUseCase consultarUseCase,
            GestionarRecursosUseCase gestionarUseCase,
            IWebHostEnvironment env)
        {
            _consultarUseCase = consultarUseCase;
            _gestionarUseCase = gestionarUseCase;
            _env = env;
        }

        // ── HELPER — extrae el rol del token JWT ──
        private RolUsuario ObtenerRolActual()
        {
            var rolClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (Enum.TryParse<RolUsuario>(rolClaim, out var rol))
                return rol;
            throw new UnauthorizedAccessException("Rol no identificado en el token.");
        }

        // ── GET ──

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "ver catálogo");

            var recursos = await _consultarUseCase.EjecutarAsync();
            return Ok(recursos);
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> BuscarPorTitulo([FromQuery] string titulo)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "buscar recursos");

            var recursos = await _consultarUseCase.BuscarPorTituloAsync(titulo);
            return Ok(recursos);
        }

        [HttpGet("categoria/{categoriaId}")]
        public async Task<IActionResult> GetPorCategoria(int categoriaId)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "ver por categoría");

            var recursos = await _consultarUseCase.BuscarPorCategoriaAsync(categoriaId);
            return Ok(recursos);
        }

        // ── POST ──

        [HttpPost("libro")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AgregarLibro([FromForm] AgregarLibroRequest request)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarRecursos(rol), "agregar libro");

            if (request == null) return BadRequest("Datos inválidos.");
            var imagenUrl = await GuardarImagenAsync(request.Imagen);
            var resultado = await _gestionarUseCase.AgregarLibroAsync(
                request.Titulo, request.Autor, request.CategoriaId, request.Stock,
                request.ISBN, request.Editorial, request.Anio, imagenUrl, request.Genero);
            return Ok(resultado);
        }

        [HttpPost("revista")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AgregarRevista([FromForm] AgregarRevistaRequest request)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarRecursos(rol), "agregar revista");

            if (request == null) return BadRequest("Datos inválidos.");
            var imagenUrl = await GuardarImagenAsync(request.Imagen);
            var resultado = await _gestionarUseCase.AgregarRevistaAsync(
                request.Titulo, request.Autor, request.CategoriaId, request.Stock,
                request.NumeroEdicion, request.ISSN, request.FechaPublicacion, imagenUrl);
            return Ok(resultado);
        }

        [HttpPost("documento")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AgregarDocumento([FromForm] AgregarDocumentoRequest request)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarRecursos(rol), "agregar documento");

            if (request == null) return BadRequest("Datos inválidos.");
            var imagenUrl = await GuardarImagenAsync(request.Imagen);
            var resultado = await _gestionarUseCase.AgregarDocumentoAsync(
                request.Titulo, request.Autor, request.CategoriaId, request.Stock,
                request.Formato, request.Institucion, request.Anio, imagenUrl);
            return Ok(resultado);
        }

        // ── PUT ──

        [HttpPut("libro/{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> EditarLibro(Guid id, [FromForm] AgregarLibroRequest request)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarRecursos(rol), "editar libro");

            if (request == null) return BadRequest("Datos inválidos.");
            var imagenUrl = await GuardarImagenAsync(request.Imagen);
            var resultado = await _gestionarUseCase.EditarLibroAsync(
                id, request.Titulo, request.Autor, request.CategoriaId, request.Stock,
                request.ISBN, request.Editorial, request.Anio, imagenUrl, request.Genero);
            return Ok(resultado);
        }

        [HttpPut("revista/{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> EditarRevista(Guid id, [FromForm] AgregarRevistaRequest request)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarRecursos(rol), "editar revista");

            if (request == null) return BadRequest("Datos inválidos.");
            var imagenUrl = await GuardarImagenAsync(request.Imagen);
            var resultado = await _gestionarUseCase.EditarRevistaAsync(
                id, request.Titulo, request.Autor, request.CategoriaId, request.Stock,
                request.NumeroEdicion, request.ISSN, request.FechaPublicacion, imagenUrl);
            return Ok(resultado);
        }

        [HttpPut("documento/{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> EditarDocumento(Guid id, [FromForm] AgregarDocumentoRequest request)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarRecursos(rol), "editar documento");

            if (request == null) return BadRequest("Datos inválidos.");
            var imagenUrl = await GuardarImagenAsync(request.Imagen);
            var resultado = await _gestionarUseCase.EditarDocumentoAsync(
                id, request.Titulo, request.Autor, request.CategoriaId, request.Stock,
                request.Formato, request.Institucion, request.Anio, imagenUrl);
            return Ok(resultado);
        }

        // ── DELETE ──

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(Guid id)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarRecursos(rol), "eliminar recurso");

            await _gestionarUseCase.EliminarRecursoAsync(id);
            return Ok("Recurso eliminado correctamente.");
        }

        // ── Helper — guarda imagen en disco ──
        private async Task<string?> GuardarImagenAsync(IFormFile? imagen)
        {
            // Ojo: Si no mandan imagen no pasa nada, devolvemos null y ya
            if (imagen == null || imagen.Length == 0) return null;

            var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(imagen.FileName).ToLowerInvariant();
            if (!extensionesPermitidas.Contains(extension)) return null;
            if (imagen.Length > 5 * 1024 * 1024) return null;

            var carpeta = Path.Combine(_env.WebRootPath ?? _env.ContentRootPath, "imagenes", "recursos");
            Directory.CreateDirectory(carpeta);

            var nombreArchivo = $"{Guid.NewGuid()}{extension}";
            var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

            using var stream = new FileStream(rutaCompleta, FileMode.Create);
            await imagen.CopyToAsync(stream);

            return $"/imagenes/recursos/{nombreArchivo}";
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
        public string? Genero { get; set; }
        public IFormFile? Imagen { get; set; }
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
        public IFormFile? Imagen { get; set; }
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
        public IFormFile? Imagen { get; set; }
    }
}
