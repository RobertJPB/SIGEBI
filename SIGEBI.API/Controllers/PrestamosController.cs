using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.DTOs;
using SIGEBI.Business.Services;
using SIGEBI.Business.UseCases.Prestamos;
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
        private readonly RegistrarPrestamoService _prestamoService;

        public PrestamosController(RegistrarPrestamoService prestamoService)
        {
            _prestamoService = prestamoService;
        }



        // Recupera el historial completo de préstamos (requiere permisos administrativos).
        [HttpGet]
        public async Task<IActionResult> GetTodos()
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerTodosLosPrestamos(rol), "ver todos los préstamos");

            var prestamos = await _prestamoService.ObtenerTodosAsync();
            return Ok(prestamos);
        }

        // Lista todos los préstamos vinculados a un usuario específico.
        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> GetPorUsuario(Guid usuarioId)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "ver préstamos por usuario");

            var prestamos = await _prestamoService.ObtenerPorUsuarioAsync(usuarioId);
            return Ok(prestamos);
        }

        // Consulta únicamente los préstamos que el usuario tiene actualmente en su posesión (sin devolver).
        [HttpGet("activos/{usuarioId}")]
        public async Task<IActionResult> GetActivosPorUsuario(Guid usuarioId)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "ver préstamos activos");

            var prestamos = await _prestamoService.ObtenerActivosPorUsuarioAsync(usuarioId);
            return Ok(prestamos);
        }

        // Identifica aquellos préstamos cuya fecha límite de devolución ya ha expirado.
        [HttpGet("atrasados")]
        public async Task<IActionResult> GetAtrasados()
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerTodosLosPrestamos(rol), "ver préstamos atrasados");

            var prestamos = await _prestamoService.ObtenerAtrasadosAsync();
            return Ok(prestamos);
        }

        // Procesa la solicitud de un nuevo préstamo de libro para un usuario.
        // Valida stock disponible, penalizaciones activas y límites de préstamos.
        [HttpPost]
        public async Task<IActionResult> SolicitarPrestamo([FromBody] PrestamoRequestDTO dto)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeSolicitarPrestamo(rol), "solicitar préstamo");

            var resultado = await _prestamoService.SolicitarPrestamoAsync(dto.UsuarioId, dto.RecursoId, dto.FechaDevolucionEstimada);
            return Ok(resultado);
        }

        // Registra el retorno físico de un libro al sistema, cerrando el ciclo del préstamo.
        [HttpPut("devolver/{prestamoId}")]
        public async Task<IActionResult> DevolverPrestamo(Guid prestamoId)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarPrestamos(rol), "devolver préstamo");

            await _prestamoService.DevolverPrestamoAsync(prestamoId);
            return Ok("Préstamo devuelto correctamente.");
        }

        // Elimina permanentemente un préstamo del historial.
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(Guid id)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGestionarPrestamos(rol), "eliminar préstamo");

            await _prestamoService.EliminarPrestamoAsync(id);
            return Ok("Préstamo eliminado correctamente.");
        }
    }
}

