using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.UseCases.Catalogo;

namespace SIGEBI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ListaDeseosController : ControllerBase
    {
        private readonly ListaDeseosUseCase _listaDeseosUseCase;

        public ListaDeseosController(ListaDeseosUseCase listaDeseosUseCase)
        {
            _listaDeseosUseCase = listaDeseosUseCase;
        }

        // Obtener lista de deseos de un usuario
        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> ObtenerPorUsuario(Guid usuarioId)
        {
            var lista = await _listaDeseosUseCase.ObtenerPorUsuarioAsync(usuarioId);
            return Ok(lista);
        }

        // Agregar recurso a la lista de deseos
        [HttpPost("usuario/{usuarioId}/recurso/{recursoId}")]
        public async Task<IActionResult> AgregarRecurso(Guid usuarioId, Guid recursoId)
        {
            await _listaDeseosUseCase.AgregarRecursoAsync(usuarioId, recursoId);
            return Ok("Recurso agregado a la lista de deseos.");
        }

        // Remover recurso de la lista de deseos
        [HttpDelete("usuario/{usuarioId}/recurso/{recursoId}")]
        public async Task<IActionResult> RemoverRecurso(Guid usuarioId, Guid recursoId)
        {
            await _listaDeseosUseCase.RemoverRecursoAsync(usuarioId, recursoId);
            return Ok("Recurso removido de la lista de deseos.");
        }
    }
}