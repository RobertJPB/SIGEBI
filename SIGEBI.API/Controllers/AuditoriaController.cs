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
    public class AuditoriaController : ControllerBase
    {
        private readonly ConsultarAuditoriaUseCase _auditoriaUseCase;

        public AuditoriaController(ConsultarAuditoriaUseCase auditoriaUseCase)
        {
            _auditoriaUseCase = auditoriaUseCase;
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

        [HttpGet]
        public async Task<IActionResult> ObtenerTodas()
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerAuditoria(rol), "ver auditoría");

            // Solo el admin deberia poder ver esto (ya esta validado arriba)
            var auditorias = await _auditoriaUseCase.ObtenerTodasAsync();
            return Ok(auditorias);
        }

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
