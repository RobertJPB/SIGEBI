using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.UseCases.Usuarios;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Domain.Enums.Seguridad;

// Controlador encargado de la gestión de multas y sanciones por retrasos en devoluciones.
// Ayuda a mantener la disciplina en el uso de los recursos bibliotecarios.
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

        // Recupera el rol para verificar permisos de visualización o aplicación de sanciones.
        private RolUsuario ObtenerRolActual()
        {
            var rolClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (Enum.TryParse<RolUsuario>(rolClaim, out var rol))
                return rol;
            throw new UnauthorizedAccessException("Rol no identificado en el token.");
        }

        // Muestra el historial de sanciones aplicadas a un usuario en particular.
        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> ObtenerPorUsuario(Guid usuarioId)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "ver penalizaciones");

            var penalizaciones = await _penalizacionesUseCase.ObtenerPenalizacionesPorUsuarioAsync(usuarioId);
            return Ok(penalizaciones);
        }

        // Ejecuta el proceso global para detectar retrasos y aplicar sanciones a todos los usuarios morosos.
        // Solo puede ser activado por personal administrativo.
        [HttpPost("aplicar")]
        public async Task<IActionResult> AplicarPenalizaciones()
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarPenalizaciones(rol), "aplicar penalizaciones");

            await _penalizacionesUseCase.AplicarPenalizacionesAsync();
            return Ok("Penalizaciones aplicadas correctamente.");
        }
    }
}
