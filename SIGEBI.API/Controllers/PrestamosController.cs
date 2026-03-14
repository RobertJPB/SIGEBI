using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.DTOs;
using SIGEBI.Business.UseCases.Prestamos;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Domain.Enums.Seguridad;

namespace SIGEBI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    // Principio SOLID (SRP - Responsabilidad Única):
    // El controlador solo orquesta la peticion HTTP. No tiene reglas de negocio.
    public class PrestamosController : ControllerBase
    {
        private readonly SolicitarPrestamoUseCase _solicitarUseCase;
        private readonly DevolverPrestamoUseCase _devolverUseCase;
        private readonly ConsultarPrestamoUseCase _consultarUseCase;

        public PrestamosController(
            SolicitarPrestamoUseCase solicitarUseCase,
            DevolverPrestamoUseCase devolverUseCase,
            ConsultarPrestamoUseCase consultarUseCase)
        {
            _solicitarUseCase = solicitarUseCase;
            _devolverUseCase = devolverUseCase;
            _consultarUseCase = consultarUseCase;
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
        public async Task<IActionResult> GetTodos()
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerTodosLosPrestamos(rol), "ver todos los préstamos");

            var prestamos = await _consultarUseCase.ObtenerTodosAsync();
            return Ok(prestamos);
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> GetPorUsuario(Guid usuarioId)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarPrestamos(rol), "ver préstamos por usuario");

            var prestamos = await _consultarUseCase.ObtenerPorUsuarioAsync(usuarioId);
            return Ok(prestamos);
        }

        [HttpGet("activos/{usuarioId}")]
        public async Task<IActionResult> GetActivosPorUsuario(Guid usuarioId)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarPrestamos(rol), "ver préstamos activos");

            var prestamos = await _consultarUseCase.ObtenerActivosPorUsuarioAsync(usuarioId);
            return Ok(prestamos);
        }

        [HttpGet("atrasados")]
        public async Task<IActionResult> GetAtrasados()
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerTodosLosPrestamos(rol), "ver préstamos atrasados");

            var prestamos = await _consultarUseCase.ObtenerAtrasadosAsync();
            return Ok(prestamos);
        }

        // ── POST ──

        [HttpPost]
        public async Task<IActionResult> SolicitarPrestamo([FromBody] PrestamoRequestDTO dto)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeSolicitarPrestamo(rol), "solicitar préstamo");

            // Aca toda la logica pesada de validacion la hace el UseCase
            var resultado = await _solicitarUseCase.EjecutarAsync(dto.UsuarioId, dto.RecursoId);
            return Ok(resultado);
        }

        // ── PUT ──

        [HttpPut("devolver/{prestamoId}")]
        public async Task<IActionResult> DevolverPrestamo(Guid prestamoId)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarPrestamos(rol), "devolver préstamo");

            await _devolverUseCase.EjecutarAsync(prestamoId);
            return Ok("Préstamo devuelto correctamente.");
        }
    }
}
