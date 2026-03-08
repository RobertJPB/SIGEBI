using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.DTOs;
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

        public AuthController(
            LoginUsuarioUseCase loginUseCase,
            RegistrarUsuarioUseCase registrarUseCase,
            RegistrarUsuarioValidator validator)
        {
            _loginUseCase = loginUseCase;
            _registrarUseCase = registrarUseCase;
            _validator = validator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UsuarioDTO dto)
        {
            var usuario = await _loginUseCase.EjecutarAsync(dto.Correo, dto.Contrasena);
            if (usuario == null)
                return Unauthorized("Correo o contraseña incorrectos.");
            return Ok(usuario);
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar(UsuarioDTO dto)
        {
            var errores = _validator.Validar(dto);
            if (errores.Any())
                return BadRequest(errores);

            await _registrarUseCase.EjecutarAsync(dto);
            return Ok("Usuario registrado exitosamente.");
        }
    }
}