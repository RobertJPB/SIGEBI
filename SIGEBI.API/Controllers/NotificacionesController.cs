using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.UseCases.Usuarios;

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

        // Obtener notificaciones de un usuario
        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> ObtenerPorUsuario(Guid usuarioId)
        {
            var notificaciones = await _notificacionesUseCase.ObtenerPorUsuarioAsync(usuarioId);
            return Ok(notificaciones);
        }

        // Marcar notificacion como leida
        [HttpPut("{notificacionId}/leida")]
        public async Task<IActionResult> MarcarComoLeida(Guid notificacionId)
        {
            await _notificacionesUseCase.MarcarComoLeidaAsync(notificacionId);
            return Ok("Notificacion marcada como leida.");
        }
    }
}