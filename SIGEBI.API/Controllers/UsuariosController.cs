using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.DTOs;
using SIGEBI.Business.UseCases.Usuarios;
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
        private readonly GestionarUsuarioUseCase _gestionarUsuario;
        private readonly RegistrarUsuarioUseCase _registrarUsuario;
        private readonly IWebHostEnvironment _env;

        public UsuariosController(
            GestionarUsuarioUseCase gestionarUsuario,
            RegistrarUsuarioUseCase registrarUsuario,
            IWebHostEnvironment env)
        {
            _gestionarUsuario = gestionarUsuario;
            _registrarUsuario = registrarUsuario;
            _env = env;
        }

        // Lista todos los usuarios registrados en la base de datos para supervisión.
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerUsuarios(rol), "ver usuarios");

            var usuarios = await _gestionarUsuario.ObtenerTodosAsync();
            return Ok(usuarios);
        }

        // Retorna la información del usuario autenticado actualmente (perfil personal).
        [HttpGet("perfil")]
        public async Task<IActionResult> ObtenerPerfil()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("No se pudo identificar al usuario desde el token.");

            var usuario = await _gestionarUsuario.ObtenerPorIdAsync(userId);
            if (usuario == null)
                return NotFound("Información de perfil no encontrada.");

            return Ok(usuario);
        }

        // Consulta la ficha detallada de un usuario específico mediante su ID.
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
        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] UsuarioDTO dto)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarUsuarios(rol), "registrar usuario");

            var id = await _registrarUsuario.EjecutarAsync(dto);
            return Ok(new { Mensaje = "Usuario registrado correctamente.", Id = id });
        }

        // Restablece el acceso a un usuario que estaba previamente desactivado o bloqueado.
        [HttpPut("{id}/activar")]
        public async Task<IActionResult> Activar(Guid id)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarUsuarios(rol), "activar usuario");

            await _gestionarUsuario.ActivarAsync(id);
            return NoContent();
        }

        // Suspende temporalmente el acceso de un usuario al sistema.
        [HttpPut("{id}/desactivar")]
        public async Task<IActionResult> Desactivar(Guid id)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarUsuarios(rol), "desactivar usuario");

            await _gestionarUsuario.DesactivarAsync(id);
            return NoContent();
        }

        // Bloquea definitivamente a un usuario (por ejemplo, por acumulación de multas o infracciones graves).
        [HttpPut("{id}/bloquear")]
        public async Task<IActionResult> Bloquear(Guid id)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarUsuarios(rol), "bloquear usuario");

            await _gestionarUsuario.BloquearAsync(id);
            return NoContent();
        }

        // Modifica el nivel de privilegios (rol) asignado a un usuario específico.
        [HttpPut("{id}/rol")]
        public async Task<IActionResult> CambiarRol(Guid id, [FromBody] int nuevoRol)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarUsuarios(rol), "cambiar rol");

            await _gestionarUsuario.CambiarRolAsync(id, (RolUsuario)nuevoRol);
            return NoContent();
        }

        // Elimina definitivamente a un usuario del sistema (hard delete).
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(Guid id)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarUsuarios(rol), "eliminar usuario");

            await _gestionarUsuario.EliminarAsync(id);
            return Ok("Usuario eliminado correctamente.");
        }

        [HttpPost("perfil/foto")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ActualizarFotoPerfil(IFormFile foto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("No se pudo identificar al usuario desde el token.");

            var imagenUrl = await GuardarImagenAsync(foto);
            if (imagenUrl == null) return BadRequest("Error al procesar la imagen.");

            await _gestionarUsuario.ActualizarImagenAsync(userId, imagenUrl);
            return Ok(new { Mensaje = "Foto de perfil actualizada.", Url = imagenUrl });
        }

        [HttpPut("perfil")]
        public async Task<IActionResult> ActualizarPerfil([FromBody] ActualizarPerfilRequest request)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("No se pudo identificar al usuario desde el token.");

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
