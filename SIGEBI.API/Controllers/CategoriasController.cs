using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.API.Extensions;
using SIGEBI.Business.Interfaces.UseCases.Catalogo;
using SIGEBI.Domain.DomainServices;

// Gestiona las categorías en las que se clasifican los recursos de la biblioteca.
// Permite organizar el catálogo por temas para facilitar la búsqueda.
namespace SIGEBI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriasUseCase _categoriasUseCase;

        public CategoriasController(ICategoriasUseCase categoriasUseCase)
        {
            _categoriasUseCase = categoriasUseCase;
        }

        // Lista todas las categorías disponibles para mostrar en el catálogo o filtros de búsqueda.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<IActionResult> ObtenerTodas()
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "ver categorías");

            var categorias = await _categoriasUseCase.ObtenerTodasAsync();
            return Ok(categorias);
        }

        // Recupera los detalles de una categoría específica mediante su identificador.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "ver categoría");

            var categoria = await _categoriasUseCase.ObtenerPorIdAsync(id);
            return Ok(categoria);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearCategoriaRequest request)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarRecursos(rol), "crear categoría");

            var categoria = await _categoriasUseCase.CrearAsync(request.Nombre);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = categoria.Id }, categoria);
        }

        // Elimina permanentemente la categoría del sistema.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarRecursos(rol), "eliminar categoría");

            await _categoriasUseCase.EliminarAsync(id);
            return NoContent();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarCategoriaRequest request)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarRecursos(rol), "actualizar categoría");

            var categoria = await _categoriasUseCase.ActualizarAsync(id, request.Nombre);
            return Ok(categoria);
        }
    }

 
    // Estas clases definen la estructura de los datos que el cliente debe enviar en las peticiones POST/PUT.

    public class CrearCategoriaRequest
    {
        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres.")]
        public string Nombre { get; set; } = string.Empty;
    }

    public class ActualizarCategoriaRequest
    {
        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres.")]
        public string Nombre { get; set; } = string.Empty;
    }
}
