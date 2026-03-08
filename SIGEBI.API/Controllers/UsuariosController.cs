using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.UseCases.Usuarios;

namespace SIGEBI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly GestionarUsuarioUseCase _gestionarUsuario;

        public UsuariosController(GestionarUsuarioUseCase gestionarUsuario)
        {
            _gestionarUsuario = gestionarUsuario;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var usuarios = await _gestionarUsuario.ObtenerTodosAsync();
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(Guid id)
        {
            var usuario = await _gestionarUsuario.ObtenerPorIdAsync(id);
            if (usuario == null)
                return NotFound("Usuario no encontrado.");
            return Ok(usuario);
        }

        [HttpPut("{id}/activar")]
        public async Task<IActionResult> Activar(Guid id)
        {
            await _gestionarUsuario.ActivarAsync(id);
            return NoContent();
        }

        [HttpPut("{id}/desactivar")]
        public async Task<IActionResult> Desactivar(Guid id)
        {
            await _gestionarUsuario.DesactivarAsync(id);
            return NoContent();
        }

        [HttpPut("{id}/bloquear")]
        public async Task<IActionResult> Bloquear(Guid id)
        {
            await _gestionarUsuario.BloquearAsync(id);
            return NoContent();
        }

        [HttpPut("{id}/rol")]
        public async Task<IActionResult> CambiarRol(Guid id, [FromBody] int nuevoRol)
        {
            await _gestionarUsuario.CambiarRolAsync(id, (Domain.Enums.Seguridad.RolUsuario)nuevoRol);
            return NoContent();
        }
    }
}