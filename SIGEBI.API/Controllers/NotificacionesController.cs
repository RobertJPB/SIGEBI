using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.UseCases.Usuarios;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Domain.Enums.Seguridad;

// Se encarga del sistema de alertas y avisos para los usuarios.
// Notifica sobre devoluciones próximas, multas aplicadas u otras novedades.
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

        // Valida la identidad y el rol para asegurar que la información sea privada y autorizada.
        private RolUsuario ObtenerRolActual()
        {
            var rolClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (Enum.TryParse<RolUsuario>(rolClaim, out var rol))
                return rol;
            throw new UnauthorizedAccessException("Rol no identificado en el token.");
        }

        // Recupera todas las notificaciones (leídas y no leídas) de un usuario.
        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> ObtenerPorUsuario(Guid usuarioId)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "ver notificaciones");

            var notificaciones = await _notificacionesUseCase.ObtenerPorUsuarioAsync(usuarioId);
            return Ok(notificaciones);
        }

        // Cambia el estado de una notificación para indicar que el usuario ya la ha visto.
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
