using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.UseCases.Usuarios;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Domain.Enums.Seguridad;

// Controlador para visualizar los registros de auditoría del sistema.
// Permite rastrear qué usuarios han realizado qué acciones en qué objetos.
namespace SIGEBI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AuditoriaController : ControllerBase
    {
        private readonly ConsultarAuditoriaUseCase _auditoriaUseCase;

        public AuditoriaController(ConsultarAuditoriaUseCase auditoriaUseCase)
        {
            _auditoriaUseCase = auditoriaUseCase;
        }

        // Método auxiliar para extraer el rol del usuario autenticado desde el token JWT.
        // Lanza una excepción si el rol no es válido o no está presente.
        private RolUsuario ObtenerRolActual()
        {
            var rolClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (Enum.TryParse<RolUsuario>(rolClaim, out var rol))
                return rol;
            throw new UnauthorizedAccessException("Rol no identificado en el token.");
        }

        // Obtiene la lista completa de todas las auditorías registradas.
        // Solo accesible para usuarios con permisos elevados (validado por AccesoPolicy).
        [HttpGet]
        public async Task<IActionResult> ObtenerTodas()
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerAuditoria(rol), "ver auditoría");

            var auditorias = await _auditoriaUseCase.ObtenerTodasAsync();
            return Ok(auditorias);
        }

        // Filtra los registros de auditoría por el ID de un usuario específico.
        // Útil para auditar el comportamiento de un integrante del sistema.
        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> ObtenerPorUsuario(Guid usuarioId)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerAuditoria(rol), "ver auditoría por usuario");

            var auditorias = await _auditoriaUseCase.ObtenerPorUsuarioAsync(usuarioId);
            return Ok(auditorias);
        }
    }
}
