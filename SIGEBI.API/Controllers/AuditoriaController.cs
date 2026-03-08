using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.UseCases.Usuarios;

namespace SIGEBI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class AuditoriaController : ControllerBase
    {
        private readonly ConsultarAuditoriaUseCase _auditoriaUseCase;

        public AuditoriaController(ConsultarAuditoriaUseCase auditoriaUseCase)
        {
            _auditoriaUseCase = auditoriaUseCase;
        }

        // Obtener todas las auditorias
        [HttpGet]
        public async Task<IActionResult> ObtenerTodas()
        {
            var auditorias = await _auditoriaUseCase.ObtenerTodasAsync();
            return Ok(auditorias);
        }

        // Obtener auditorias de un usuario
        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> ObtenerPorUsuario(Guid usuarioId)
        {
            var auditorias = await _auditoriaUseCase.ObtenerPorUsuarioAsync(usuarioId);
            return Ok(auditorias);
        }
    }
}