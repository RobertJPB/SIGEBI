using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces.Services;
using SIGEBI.Business.Interfaces.UseCases.Usuarios;
using SIGEBI.Business.Validators;

// Este controlador gestiona los procesos de autenticación y registro de nuevos usuarios.
// Es la puerta de entrada para obtener acceso seguro al sistema a través de JWT.
namespace SIGEBI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILoginUsuarioUseCase _loginUseCase;
        private readonly IRegistrarUsuarioUseCase _registrarUseCase;
        private readonly RegistrarUsuarioValidator _validator;
        private readonly IJwtService _jwtService;

        public AuthController(
            ILoginUsuarioUseCase loginUseCase,
            IRegistrarUsuarioUseCase registrarUseCase,
            RegistrarUsuarioValidator validator,
            IJwtService jwtService)
        {
            _loginUseCase = loginUseCase;
            _registrarUseCase = registrarUseCase;
            _validator = validator;
            _jwtService = jwtService;
        }

        // Procesa el inicio de sesión del usuario.
        // Si las credenciales son válidas, genera y retorna un token JWT para autorizar futuras peticiones.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UsuarioDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Correo) || string.IsNullOrWhiteSpace(dto.Contrasena))
                return BadRequest("Correo y contraseña son obligatorios.");

            var usuario = await _loginUseCase.EjecutarAsync(dto.Correo, dto.Contrasena);

            if (usuario == null)
                return Unauthorized("Correo o contraseña incorrectos.");

            var token = _jwtService.GenerarToken(usuario);

            return Ok(new
            {
                token = token
            });
        }

        // Registra un nuevo usuario aplicando las validaciones correspondientes (formato de correo, etc.).
        // Devuelve éxito una vez que los datos han sido persistidos correctamente.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] UsuarioDTO dto)
        {
            if (dto == null)
                return BadRequest("Datos inválidos.");

            // Verifica que el DTO cumpla con las reglas de negocio definidas.
            var errores = _validator.Validar(dto);

            if (errores.Any())
                return BadRequest(errores);

            // Delega la creación del usuario al caso de uso correspondiente.
            var id = await _registrarUseCase.EjecutarAsync(dto);

            return Ok(new { Mensaje = "Usuario registrado exitosamente.", Id = id });
        }
    }
}
