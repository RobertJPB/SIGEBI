using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.UseCases.Usuarios;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Domain.Enums.Seguridad;

namespace SIGEBI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PenalizacionesController : ControllerBase
    {
        private readonly PenalizacionesUseCase _penalizacionesUseCase;

        public PenalizacionesController(PenalizacionesUseCase penalizacionesUseCase)
        {
            _penalizacionesUseCase = penalizacionesUseCase;
        }

        // ── HELPER ──
        private RolUsuario ObtenerRolActual()
        {
            var rolClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (Enum.TryParse<RolUsuario>(rolClaim, out var rol))
                return rol;
            throw new UnauthorizedAccessException("Rol no identificado en el token.");
        }

        // ── GET ──

        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> ObtenerPorUsuario(Guid usuarioId)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "ver penalizaciones");

            var penalizaciones = await _penalizacionesUseCase.ObtenerPenalizacionesPorUsuarioAsync(usuarioId);
            return Ok(penalizaciones);
        }

        // ── POST ──

        [HttpPost("aplicar")]
        public async Task<IActionResult> AplicarPenalizaciones()
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarPenalizaciones(rol), "aplicar penalizaciones");

            // Llama al caso de uso que procesa a todos los morosos de una
            await _penalizacionesUseCase.AplicarPenalizacionesAsync();
            return Ok("Penalizaciones aplicadas correctamente.");
        }
    }
}
