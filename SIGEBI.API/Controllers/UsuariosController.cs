using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.DTOs;
using SIGEBI.Business.UseCases.Usuarios;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Domain.Enums.Seguridad;

namespace SIGEBI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly GestionarUsuarioUseCase _gestionarUsuario;
        private readonly RegistrarUsuarioUseCase _registrarUsuario;

        public UsuariosController(
            GestionarUsuarioUseCase gestionarUsuario,
            RegistrarUsuarioUseCase registrarUsuario)
        {
            _gestionarUsuario = gestionarUsuario;
            _registrarUsuario = registrarUsuario;
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
        public async Task<IActionResult> ObtenerTodos()
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerUsuarios(rol), "ver usuarios");

            var usuarios = await _gestionarUsuario.ObtenerTodosAsync();
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(Guid id)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "ver usuario");

            var usuario = await _gestionarUsuario.ObtenerPorIdAsync(id);
            if (usuario == null)
                return NotFound("Usuario no encontrado.");
            return Ok(usuario);
        }

        // ── POST ──

        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] UsuarioDTO dto)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarUsuarios(rol), "registrar usuario");

            // El UseCase ya se encarga de todo el hash de la contraseña y demas
            await _registrarUsuario.EjecutarAsync(dto);
            return Ok("Usuario registrado correctamente.");
        }

        // ── PUT ──

        [HttpPut("{id}/activar")]
        public async Task<IActionResult> Activar(Guid id)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarUsuarios(rol), "activar usuario");

            await _gestionarUsuario.ActivarAsync(id);
            return NoContent();
        }

        [HttpPut("{id}/desactivar")]
        public async Task<IActionResult> Desactivar(Guid id)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarUsuarios(rol), "desactivar usuario");

            await _gestionarUsuario.DesactivarAsync(id);
            return NoContent();
        }

        [HttpPut("{id}/bloquear")]
        public async Task<IActionResult> Bloquear(Guid id)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarUsuarios(rol), "bloquear usuario");

            await _gestionarUsuario.BloquearAsync(id);
            return NoContent();
        }

        [HttpPut("{id}/rol")]
        public async Task<IActionResult> CambiarRol(Guid id, [FromBody] int nuevoRol)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarUsuarios(rol), "cambiar rol");

            await _gestionarUsuario.CambiarRolAsync(id, (RolUsuario)nuevoRol);
            return NoContent();
        }
    }
}
