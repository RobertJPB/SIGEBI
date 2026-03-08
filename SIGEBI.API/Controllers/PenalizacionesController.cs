using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.UseCases.Usuarios;

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

        // Obtener penalizaciones de un usuario
        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> ObtenerPorUsuario(Guid usuarioId)
        {
            var penalizaciones = await _penalizacionesUseCase.ObtenerPenalizacionesPorUsuarioAsync(usuarioId);
            return Ok(penalizaciones);
        }

        // Aplicar penalizaciones a prestamos atrasados (solo administrador y bibliotecario)
        [HttpPost("aplicar")]
        [Authorize(Roles = "Administrador,Bibliotecario")]
        public async Task<IActionResult> AplicarPenalizaciones()
        {
            await _penalizacionesUseCase.AplicarPenalizacionesAsync();
            return Ok("Penalizaciones aplicadas correctamente.");
        }
    }
}