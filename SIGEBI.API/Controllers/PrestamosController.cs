using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces.UseCases.Prestamos;
using SIGEBI.Business.Validators;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Domain.Enums.Seguridad;
using SIGEBI.API.Extensions;

// Este controlador orquesta todas las solicitudes relacionadas con el préstamo y devolución de libros.
// Actúa como el puente entre las peticiones HTTP y los servicios de lógica de negocio de préstamos.
namespace SIGEBI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PrestamosController : ControllerBase
    {
        private readonly ISolicitarPrestamoUseCase _solicitarUseCase;
        private readonly IDevolverPrestamoUseCase _devolverUseCase;
        private readonly IConsultarPrestamoUseCase _consultarUseCase;
        private readonly IEliminarPrestamoUseCase _eliminarUseCase;
        private readonly SolicitarPrestamoValidator _validator;

        public PrestamosController(
            ISolicitarPrestamoUseCase solicitarUseCase,
            IDevolverPrestamoUseCase devolverUseCase,
            IConsultarPrestamoUseCase consultarUseCase,
            IEliminarPrestamoUseCase eliminarUseCase,
            SolicitarPrestamoValidator validator)
        {
            _solicitarUseCase = solicitarUseCase;
            _devolverUseCase = devolverUseCase;
            _consultarUseCase = consultarUseCase;
            _eliminarUseCase = eliminarUseCase;
            _validator = validator;
        }



        // Recupera el historial completo de préstamos (requiere permisos administrativos).
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerTodosLosPrestamos(rol), "ver todos los préstamos");

            var prestamos = await _consultarUseCase.ObtenerTodosAsync();
            return Ok(prestamos);
        }

        // Lista todos los préstamos vinculados a un usuario específico.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> ObtenerPorUsuario(Guid usuarioId)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "ver préstamos por usuario");

            var prestamos = await _consultarUseCase.ObtenerPorUsuarioAsync(usuarioId);
            return Ok(prestamos);
        }

        // Consulta únicamente los préstamos que el usuario tiene actualmente en su posesión (sin devolver).
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("activos/{usuarioId}")]
        public async Task<IActionResult> ObtenerActivosPorUsuario(Guid usuarioId)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "ver préstamos activos");

            var prestamos = await _consultarUseCase.ObtenerActivosPorUsuarioAsync(usuarioId);
            return Ok(prestamos);
        }

        // Identifica aquellos préstamos cuya fecha límite de devolución ya ha expirado.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("atrasados")]
        public async Task<IActionResult> ObtenerAtrasados()
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerTodosLosPrestamos(rol), "ver préstamos atrasados");

            var prestamos = await _consultarUseCase.ObtenerAtrasadosAsync();
            return Ok(prestamos);
        }

        // Procesa la solicitud de un nuevo préstamo de libro para un usuario.
        // Valida stock disponible, penalizaciones activas y límites de préstamos.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> SolicitarPrestamo([FromBody] PrestamoRequestDTO dto)
        {
            try
            {
                var rol = User.ObtenerRolActual();
                AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeSolicitarPrestamo(rol), "solicitar préstamo");

                // Validación de datos básicos de la solicitud
                var errores = _validator.Validar(dto);
                if (errores.Any()) return BadRequest(new { errores });

                var resultado = await _solicitarUseCase.EjecutarAsync(dto.UsuarioId, dto.RecursoId, dto.FechaDevolucionEstimada);
                return Ok(resultado);
            }
            catch (InvalidOperationException ex)
            {
                // Este catch captura errores de lógica de negocio (ej: límite de préstamos excedido)
                return BadRequest(new { message = ex.Message });
            }
        }

        // Registra el retorno físico de un libro al sistema, cerrando el ciclo del préstamo.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("devolver/{prestamoId}")]
        public async Task<IActionResult> DevolverPrestamo(Guid prestamoId)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarPrestamos(rol), "devolver préstamo");

            await _devolverUseCase.EjecutarAsync(prestamoId);
            return Ok(new { mensaje = "Préstamo devuelto correctamente." });
        }

        // Elimina permanentemente un préstamo del historial.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(Guid id)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarPrestamos(rol), "eliminar préstamo");

            await _eliminarUseCase.EjecutarAsync(id);
            return Ok(new { mensaje = "Préstamo eliminado correctamente." });
        }
    }
}

