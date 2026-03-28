using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.UseCases.Usuarios;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Domain.Enums.Seguridad;
using SIGEBI.API.Extensions;

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



        private Guid ObtenerUsuarioIdActual()
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(idClaim, out var id))
                return id;
            throw new UnauthorizedAccessException("Usuario no identificado en el token.");
        }

        // Recupera todas las notificaciones (leídas y no leídas) de un usuario.
        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> ObtenerPorUsuario(Guid usuarioId)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "ver notificaciones");

            var notificaciones = await _notificacionesUseCase.ObtenerPorUsuarioAsync(usuarioId);
            return Ok(notificaciones);
        }

        [HttpGet("usuario/{usuarioId}/count")]
        public async Task<IActionResult> ObtenerCantPendientes(Guid usuarioId)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "ver conteo de notificaciones");

            var count = await _notificacionesUseCase.ObtenerCantPendientesAsync(usuarioId);
            return Ok(count);
        }

        [HttpPut("{notificacionId}/leida")]
        public async Task<IActionResult> MarcarComoLeida(Guid notificacionId)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "marcar notificación como leída");

            await _notificacionesUseCase.MarcarComoLeidaAsync(notificacionId);
            return Ok("Notificación marcada como leída.");
        }

        // Elimina una notificación. El usuario solo puede borrar las suyas.
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(Guid id)
        {
            var rol = User.ObtenerRolActual();
            var usuarioId = ObtenerUsuarioIdActual();

            var notificacion = await _notificacionesUseCase.ObtenerPorIdAsync(id);

            if (notificacion.UsuarioId != usuarioId && !AccesoPolicy.PuedeGestionarUsuarios(rol))
            {
                return Forbid("No tienes permiso para eliminar esta notificación.");
            }

            await _notificacionesUseCase.EliminarNotificacionAsync(id);
            return Ok("Notificación eliminada correctamente.");
        }
    }
}
