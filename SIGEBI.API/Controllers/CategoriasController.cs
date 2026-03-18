using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.UseCases.Catalogo;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Domain.Enums.Seguridad;

// Gestiona las categorías en las que se clasifican los recursos de la biblioteca.
// Permite organizar el catálogo por temas para facilitar la búsqueda.
namespace SIGEBI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriasController : ControllerBase
    {
        private readonly CategoriasUseCase _categoriasUseCase;

        public CategoriasController(CategoriasUseCase categoriasUseCase)
        {
            _categoriasUseCase = categoriasUseCase;
        }

        // Obtiene el rol del usuario conectado para validar sus permisos de acceso.
        private RolUsuario ObtenerRolActual()
        {
            var rolClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (Enum.TryParse<RolUsuario>(rolClaim, out var rol))
                return rol;
            throw new UnauthorizedAccessException("Rol no identificado en el token.");
        }

        // Lista todas las categorías disponibles para mostrar en el catálogo o filtros de búsqueda.
        [HttpGet]
        public async Task<IActionResult> ObtenerTodas()
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "ver categorías");

            var categorias = await _categoriasUseCase.ObtenerTodasAsync();
            return Ok(categorias);
        }

        // Recupera los detalles de una categoría específica mediante su identificador.
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "ver categoría");

            var categoria = await _categoriasUseCase.ObtenerPorIdAsync(id);
            return Ok(categoria);
        }

        // Crea una nueva categoría temática.
        // Requiere privilegios de gestión de recursos (bibliotecario o administrador).
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] string nombre)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarRecursos(rol), "crear categoría");

            var categoria = await _categoriasUseCase.CrearAsync(nombre);
            return Ok(categoria);
        }
    }
}
