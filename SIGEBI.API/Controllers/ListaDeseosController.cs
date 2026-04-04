using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.Interfaces.UseCases.Catalogo;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Domain.Enums.Seguridad;
using SIGEBI.API.Extensions;

// Administra las listas de deseos personales de los usuarios.
// Permite que los usuarios guarden recursos que les interesan para futuras solicitudes.
namespace SIGEBI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ListaDeseosController : ControllerBase
    {
        private readonly IListaDeseosUseCase _listaDeseosUseCase;

        public ListaDeseosController(IListaDeseosUseCase listaDeseosUseCase)
        {
            _listaDeseosUseCase = listaDeseosUseCase;
        }

        // Lista todos los recursos que un usuario específico tiene en su lista de deseos.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> ObtenerPorUsuario(Guid usuarioId)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "ver lista de deseos");

            var lista = await _listaDeseosUseCase.ObtenerPorUsuarioAsync(usuarioId);
            return Ok(lista);
        }

        // Añade un recurso específico a la lista de deseos del usuario.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("usuario/{usuarioId}/recurso/{recursoId}")]
        public async Task<IActionResult> AgregarRecurso(Guid usuarioId, Guid recursoId)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "agregar a lista de deseos");

            await _listaDeseosUseCase.AgregarRecursoAsync(usuarioId, recursoId);
            return Ok(new { message = "Recurso agregado a la lista de deseos." });
        }

        // Elimina un recurso de la lista de deseos del usuario.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("usuario/{usuarioId}/recurso/{recursoId}")]
        public async Task<IActionResult> RemoverRecurso(Guid usuarioId, Guid recursoId)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "remover de lista de deseos");

            await _listaDeseosUseCase.RemoverRecursoAsync(usuarioId, recursoId);
            return Ok(new { message = "Recurso removido de la lista de deseos." });
        }
    }
}
