using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.UseCases.Catalogo;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Domain.Enums.Seguridad;

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

        // ── HELPER ──
        private RolUsuario ObtenerRolActual()
        {
            var rolClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (Enum.TryParse<RolUsuario>(rolClaim, out var rol))
                return rol;
            throw new UnauthorizedAccessException("Rol no identificado en el token.");
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodas()
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "ver categorías");

            var categorias = await _categoriasUseCase.ObtenerTodasAsync();
            return Ok(categorias);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "ver categoría");

            var categoria = await _categoriasUseCase.ObtenerPorIdAsync(id);
            return Ok(categoria);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] string nombre)
        {
            var rol = ObtenerRolActual();
            // TODO: Mover la creacion de categorias solo a Administradores?
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarRecursos(rol), "crear categoría");

            var categoria = await _categoriasUseCase.CrearAsync(nombre);
            return Ok(categoria);
        }
    }
}
