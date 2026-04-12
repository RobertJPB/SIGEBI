using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces.UseCases.Usuarios;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Domain.Enums.Seguridad;
using SIGEBI.API.Extensions;

// Administra el ciclo de vida de los usuarios en el sistema (registro, activación, bloqueo y roles).
// Permite que el personal administrativo controle quién tiene acceso a la plataforma.
namespace SIGEBI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly IGestionarUsuarioUseCase _gestionarUsuario;
        private readonly IRegistrarUsuarioUseCase _registrarUsuario;
        private readonly IWebHostEnvironment _env;

        public UsuariosController(
            IGestionarUsuarioUseCase gestionarUsuario,
            IRegistrarUsuarioUseCase registrarUsuario,
            IWebHostEnvironment env)
        {
            _gestionarUsuario = gestionarUsuario;
            _registrarUsuario = registrarUsuario;
            _env = env;
        }

        // Lista todos los usuarios registrados en la base de datos para supervisión.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerUsuarios(rol), "ver usuarios");

            var usuarios = await _gestionarUsuario.ObtenerTodosAsync();
            return Ok(usuarios);
        }

        // Retorna la información del usuario autenticado actualmente (perfil personal).
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("perfil")]
        public async Task<IActionResult> ObtenerPerfil()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "No se pudo identificar al usuario desde el token." });

            var usuario = await _gestionarUsuario.ObtenerPorIdAsync(userId);
            if (usuario == null)
                return NotFound("Información de perfil no encontrada.");

            return Ok(usuario);
        }

        // Consulta la ficha detallada de un usuario específico mediante su ID.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(Guid id)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "ver usuario");

            var usuario = await _gestionarUsuario.ObtenerPorIdAsync(id);
            if (usuario == null)
                return NotFound("Usuario no encontrado.");
            return Ok(usuario);
        }

        // Registra manualmente un nuevo usuario, útil para dar de alta personal interno por parte de administradores.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] UsuarioDTO dto)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarUsuarios(rol), "registrar usuario");

            var id = await _registrarUsuario.EjecutarAsync(dto);
            return Ok(new { Mensaje = "Usuario registrado correctamente.", Id = id });
        }

        // Restablece el acceso a un usuario que estaba previamente desactivado o bloqueado.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("{id}/activar")]
        public async Task<IActionResult> Activar(Guid id)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarUsuarios(rol), "activar usuario");

            await _gestionarUsuario.ActivarAsync(id);
            return NoContent();
        }

        // Suspende temporalmente el acceso de un usuario al sistema.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("{id}/desactivar")]
        public async Task<IActionResult> Desactivar(Guid id, [FromBody] SIGEBI.Business.DTOs.MotivoRequest request)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarUsuarios(rol), "desactivar usuario");

            if (string.IsNullOrWhiteSpace(request?.Motivo)) return BadRequest("El motivo de desactivación es obligatorio.");

            await _gestionarUsuario.DesactivarAsync(id, request.Motivo);
            return NoContent();
        }

        // Suspende el acceso de un usuario.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("{id}/suspender")]
        public async Task<IActionResult> Suspender(Guid id, [FromBody] SIGEBI.Business.DTOs.MotivoRequest request)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarUsuarios(rol), "suspender usuario");

            if (string.IsNullOrWhiteSpace(request?.Motivo)) return BadRequest("El motivo de suspensión es obligatorio.");

            await _gestionarUsuario.SuspenderAsync(id, request.Motivo, request.TiempoDias);
            return NoContent();
        }

        // Bloquea definitivamente a un usuario (por ejemplo, por acumulación de multas o infracciones graves).
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("{id}/bloquear")]
        public async Task<IActionResult> Bloquear(Guid id, [FromBody] SIGEBI.Business.DTOs.MotivoRequest request)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarUsuarios(rol), "bloquear usuario");

            if (string.IsNullOrWhiteSpace(request?.Motivo)) return BadRequest("El motivo de bloqueo es obligatorio.");

            await _gestionarUsuario.BloquearAsync(id, request.Motivo);
            return NoContent();
        }

        // Modifica el nivel de privilegios (rol) asignado a un usuario específico.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("{id}/rol")]
        public async Task<IActionResult> CambiarRol(Guid id, [FromBody] int nuevoRol)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarUsuarios(rol), "cambiar rol");

            await _gestionarUsuario.CambiarRolAsync(id, (RolUsuario)nuevoRol);
            return NoContent();
        }

        // Elimina definitivamente a un usuario del sistema (hard delete).
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(Guid id)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarUsuarios(rol), "eliminar usuario");

            await _gestionarUsuario.EliminarAsync(id);
            return Ok("Usuario eliminado correctamente.");
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("perfil/foto")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ActualizarFotoPerfil(IFormFile foto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "No se pudo identificar al usuario desde el token." });

            var imagenUrl = await GuardarImagenAsync(foto);
            if (imagenUrl == null) return BadRequest("Error al procesar la imagen.");

            await _gestionarUsuario.ActualizarImagenAsync(userId, imagenUrl);
            return Ok(new { Mensaje = "Foto de perfil actualizada.", Url = imagenUrl });
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("perfil")]
        public async Task<IActionResult> ActualizarPerfil([FromBody] ActualizarPerfilRequest request)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "No se pudo identificar al usuario desde el token." });

            await _gestionarUsuario.ActualizarPerfilAsync(userId, request.Nombre, request.Correo);
            return Ok(new { Mensaje = "Perfil actualizado correctamente." });
        }

        private async Task<string?> GuardarImagenAsync(IFormFile? imagen)
        {
            if (imagen == null || imagen.Length == 0) return null;
            var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(imagen.FileName).ToLowerInvariant();
            if (!extensionesPermitidas.Contains(extension)) return null;

            var carpeta = Path.Combine(_env.ContentRootPath, "imagenes", "usuarios");
            Directory.CreateDirectory(carpeta);
            var nombreArchivo = $"{Guid.NewGuid()}{extension}";
            var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

            using var stream = new FileStream(rutaCompleta, FileMode.Create);
            await imagen.CopyToAsync(stream);
            return $"/imagenes/usuarios/{nombreArchivo}";
        }
    }
}
