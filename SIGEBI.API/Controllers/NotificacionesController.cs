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
    public class NotificacionesController : ControllerBase
    {
        private readonly NotificacionesUseCase _notificacionesUseCase;

        public NotificacionesController(NotificacionesUseCase notificacionesUseCase)
        {
            _notificacionesUseCase = notificacionesUseCase;
        }

        // ── HELPER ──
        private RolUsuario ObtenerRolActual()
        {
            var rolClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (Enum.TryParse<RolUsuario>(rolClaim, out var rol))
                return rol;
            throw new UnauthorizedAccessException("Rol no identificado en el token.");
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> ObtenerPorUsuario(Guid usuarioId)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "ver notificaciones");

            var notificaciones = await _notificacionesUseCase.ObtenerPorUsuarioAsync(usuarioId);
            return Ok(notificaciones);
        }

        [HttpPut("{notificacionId}/leida")]
        public async Task<IActionResult> MarcarComoLeida(Guid notificacionId)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "marcar notificación como leída");

            await _notificacionesUseCase.MarcarComoLeidaAsync(notificacionId);
            return Ok("Notificación marcada como leída.");
        }
    }
}
