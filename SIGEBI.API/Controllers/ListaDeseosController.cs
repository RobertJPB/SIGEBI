using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.UseCases.Catalogo;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Domain.Enums.Seguridad;

// Administra las listas de deseos personales de los usuarios.
// Permite que los usuarios guarden recursos que les interesan para futuras solicitudes.
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

        // Obtiene el rol del token para aplicar las políticas de acceso correspondientes.
        private RolUsuario ObtenerRolActual()
        {
            var rolClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (Enum.TryParse<RolUsuario>(rolClaim, out var rol))
                return rol;
            throw new UnauthorizedAccessException("Rol no identificado en el token.");
        }

        // Lista todos los recursos que un usuario específico tiene en su lista de deseos.
        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> ObtenerPorUsuario(Guid usuarioId)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "ver lista de deseos");

            var lista = await _listaDeseosUseCase.ObtenerPorUsuarioAsync(usuarioId);
            return Ok(lista);
        }

        // Añade un recurso específico a la lista de deseos del usuario.
        [HttpPost("usuario/{usuarioId}/recurso/{recursoId}")]
        public async Task<IActionResult> AgregarRecurso(Guid usuarioId, Guid recursoId)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "agregar a lista de deseos");

            await _listaDeseosUseCase.AgregarRecursoAsync(usuarioId, recursoId);
            return Ok("Recurso agregado a la lista de deseos.");
        }

        // Elimina un recurso de la lista de deseos del usuario.
        [HttpDelete("usuario/{usuarioId}/recurso/{recursoId}")]
        public async Task<IActionResult> RemoverRecurso(Guid usuarioId, Guid recursoId)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "remover de lista de deseos");

            await _listaDeseosUseCase.RemoverRecursoAsync(usuarioId, recursoId);
            return Ok("Recurso removido de la lista de deseos.");
        }
    }
}
