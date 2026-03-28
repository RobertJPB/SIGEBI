using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.UseCases.Usuarios;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Domain.Enums.Seguridad;
using SIGEBI.API.Extensions;

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



        // Obtiene la lista completa de todas las auditorías registradas.
        // Solo accesible para usuarios con permisos elevados (validado por AccesoPolicy).
        [HttpGet]
        public async Task<IActionResult> ObtenerTodas()
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerAuditoria(rol), "ver auditoría");

            var auditorias = await _auditoriaUseCase.ObtenerTodasAsync();
            return Ok(auditorias);
        }

        // Filtra los registros de auditoría por el ID de un usuario específico.
        // Útil para auditar el comportamiento de un integrante del sistema.
        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> ObtenerPorUsuario(Guid usuarioId)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerAuditoria(rol), "ver auditoría por usuario");

            var auditorias = await _auditoriaUseCase.ObtenerPorUsuarioAsync(usuarioId);
            return Ok(auditorias);
        }


    }
}
