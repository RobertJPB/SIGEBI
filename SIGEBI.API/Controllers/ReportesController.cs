using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.Services;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Domain.Enums.Seguridad;
using SIGEBI.API.Extensions;

// Provee herramientas de análisis y reporte sobre el estado de la biblioteca.
// Permite exportar datos consolidados para la toma de decisiones administrativa.
namespace SIGEBI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportesController : ControllerBase
    {
        private readonly GenerarReportesService _reportesService;

        public ReportesController(GenerarReportesService reportesService)
        {
            _reportesService = reportesService;
        }



        // ── GET ──

        // Genera un resumen estadístico global: total de libros, usuarios activos, préstamos vigentes, etc.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("general")]
        public async Task<IActionResult> ObtenerReporteGeneral()
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGenerarReportes(rol), "generar reporte general");

            // TODO: Agregar filtros por fecha de ser necesario mas adelante
            var reporte = await _reportesService.GenerarReporteGeneralAsync();
            return Ok(reporte);
        }

        // Recupera la relación detallada de préstamos realizados dentro de un intervalo de tiempo específico.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("prestamos")]
        public async Task<IActionResult> ObtenerPrestamosPorPeriodo(
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGenerarReportes(rol), "generar reporte de préstamos");

            var prestamos = await _reportesService.ObtenerPrestamosPorPeriodoAsync(fechaInicio, fechaFin);
            return Ok(prestamos);
        }

        // Identifica usuarios con historial recurrente de incumplimiento.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("usuarios-morosos")]
        public async Task<IActionResult> ObtenerUsuariosMasPenalizados([FromQuery] int top = 5)
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGenerarReportes(rol), "ver reporte de usuarios morosos");

            var usuarios = await _reportesService.ObtenerUsuariosMasPenalizadosAsync(top);
            return Ok(usuarios);
        }

        // Lista de penalizaciones vigentes en toda la biblioteca.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("penalizaciones-activas")]
        public async Task<IActionResult> ObtenerPenalizacionesActivas()
        {
            var rol = User.ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGenerarReportes(rol), "ver reporte de penalizaciones activas");

            var penalizaciones = await _reportesService.ObtenerPenalizacionesActivasAsync();
            return Ok(penalizaciones);
        }
    }
}