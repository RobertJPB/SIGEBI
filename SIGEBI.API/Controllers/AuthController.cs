using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces.Services;
using SIGEBI.Business.UseCases.Usuarios;
using SIGEBI.Business.Validators;

namespace SIGEBI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly LoginUsuarioUseCase _loginUseCase;
        private readonly RegistrarUsuarioUseCase _registrarUseCase;
        private readonly RegistrarUsuarioValidator _validator;
        private readonly IJwtService _jwtService;

        public AuthController(
            LoginUsuarioUseCase loginUseCase,
            RegistrarUsuarioUseCase registrarUseCase,
            RegistrarUsuarioValidator validator,
            IJwtService jwtService)
        {
            _loginUseCase = loginUseCase;
            _registrarUseCase = registrarUseCase;
            _validator = validator;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UsuarioDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Correo) || string.IsNullOrWhiteSpace(dto.Contrasena))
                return BadRequest("Correo y contraseña son obligatorios.");

            var usuario = await _loginUseCase.EjecutarAsync(dto.Correo, dto.Contrasena);

            // Si falla el login no damos muchos detalles por seguridad
            if (usuario == null)
                return Unauthorized("Correo o contraseña incorrectos.");

            // Generación del token JWT con los claims del usuario (ID, Rol, etc.)
            var token = _jwtService.GenerarToken(usuario);

            return Ok(new
            {
                token = token
            });
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] UsuarioDTO dto)
        {
            if (dto == null)
                return BadRequest("Datos inválidos.");

            var errores = _validator.Validar(dto);

            if (errores.Any())
                return BadRequest(errores);

            await _registrarUseCase.EjecutarAsync(dto);

            return Ok("Usuario registrado exitosamente.");
        }
    }
}