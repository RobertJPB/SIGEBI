using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.DTOs;
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

        // Muestra todas las penalizaciones del sistema (Admin/Bibliotecario).
        [HttpGet]
        public async Task<IActionResult> ObtenerTodas()
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarPenalizaciones(rol), "ver todas las penalizaciones");

            var penalizaciones = await _penalizacionesUseCase.ObtenerTodasLasPenalizacionesAsync();
            return Ok(penalizaciones);
        }

        // Muestra el historial de sanciones aplicadas a un usuario en particular.
        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> ObtenerPorUsuario(Guid usuarioId)
        {
            var rol = ObtenerRolActual();
            // Cambiamos el permiso para que sea ver usuarios o gestionar penalizaciones
            bool puedeVer = AccesoPolicy.PuedeVerUsuarios(rol) || AccesoPolicy.PuedeGestionarPenalizaciones(rol);
            AccesoPolicy.ValidarAcceso(rol, puedeVer, "ver penalizaciones de un usuario");

            var penalizaciones = await _penalizacionesUseCase.ObtenerPenalizacionesPorUsuarioAsync(usuarioId);
            return Ok(penalizaciones);
        }

        // Aplica una penalización manual (POST general con DTO completo).
        [HttpPost]
        public async Task<IActionResult> Aplicar([FromBody] AplicarPenalizacionManualDTO dto)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarPenalizaciones(rol), "aplicar penalización manual");

            await _penalizacionesUseCase.AplicarPenalizacionManualAsync(dto);
            return Ok("Penalización aplicada correctamente.");
        }

        // Aplica una penalización manual a un usuario específico (Legacy / URL based).
        [HttpPost("usuario/{usuarioId}")]
        public async Task<IActionResult> AplicarManual(Guid usuarioId, [FromBody] AplicarPenalizacionManualDTO dto)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarPenalizaciones(rol), "aplicar penalización manual");

            dto.UsuarioId = usuarioId;
            await _penalizacionesUseCase.AplicarPenalizacionManualAsync(dto);
            return Ok("Penalización aplicada correctamente.");
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

        // Elimina una penalización del sistema. Solo para casos de error administrativo.
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(Guid id)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarPenalizaciones(rol), "eliminar penalización");

            await _penalizacionesUseCase.EliminarPenalizacionAsync(id);
            return Ok("Penalización eliminada correctamente.");
        }

        // Finaliza manualmente una penalización activa (por ejemplo, tras el pago de una multa).
        [HttpPatch("{id}/finalizar")]
        public async Task<IActionResult> Finalizar(Guid id)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarPenalizaciones(rol), "finalizar penalización");

            await _penalizacionesUseCase.FinalizarPenalizacionAsync(id);
            return Ok("Penalización finalizada correctamente.");
        }
    }
}
