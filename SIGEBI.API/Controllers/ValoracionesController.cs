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
    public class ValoracionesController : ControllerBase
    {
        private readonly ValoracionesUseCase _valoracionesUseCase;

        public ValoracionesController(ValoracionesUseCase valoracionesUseCase)
        {
            _valoracionesUseCase = valoracionesUseCase;
        }

        // ── HELPER ──
        private RolUsuario ObtenerRolActual()
        {
            var rolClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (Enum.TryParse<RolUsuario>(rolClaim, out var rol))
                return rol;
            throw new UnauthorizedAccessException("Rol no identificado en el token.");
        }

        [HttpGet("recurso/{recursoId}")]
        public async Task<IActionResult> ObtenerPorRecurso(Guid recursoId)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "ver valoraciones");

            var valoraciones = await _valoracionesUseCase.ObtenerValoracionesPorRecursoAsync(recursoId);
            return Ok(valoraciones);
        }

        [HttpGet("recurso/{recursoId}/promedio")]
        public async Task<IActionResult> ObtenerPromedio(Guid recursoId)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "ver promedio de valoraciones");

            var promedio = await _valoracionesUseCase.ObtenerPromedioAsync(recursoId);
            return Ok(promedio);
        }

        [HttpPost]
        public async Task<IActionResult> Agregar([FromBody] ValoracionDTO dto)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeSolicitarPrestamo(rol), "agregar valoración");

            var valoracion = await _valoracionesUseCase.AgregarValoracionAsync(
                dto.UsuarioId, dto.RecursoId, dto.Calificacion, dto.Comentario);
            return Ok(valoracion);
        }
    }
}
