using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.Services;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Domain.Enums.Seguridad;

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

        // ── HELPER ──
        // Obtiene el rol para asegurar que solo personal autorizado pueda generar informes de gestión.
        private RolUsuario ObtenerRolActual()
        {
            var rolClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (Enum.TryParse<RolUsuario>(rolClaim, out var rol))
                return rol;
            throw new UnauthorizedAccessException("Rol no identificado en el token.");
        }

        // ── GET ──

        // Genera un resumen estadístico global: total de libros, usuarios activos, préstamos vigentes, etc.
        [HttpGet("general")]
        public async Task<IActionResult> ObtenerReporteGeneral()
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGenerarReportes(rol), "generar reporte general");

            // TODO: Agregar filtros por fecha de ser necesario mas adelante
            var reporte = await _reportesService.GenerarReporteGeneralAsync();
            return Ok(reporte);
        }

        // Recupera la relación detallada de préstamos realizados dentro de un intervalo de tiempo específico.
        [HttpGet("prestamos")]
        public async Task<IActionResult> ObtenerPrestamosPorPeriodo(
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeGenerarReportes(rol), "generar reporte de préstamos");

            var prestamos = await _reportesService.ObtenerPrestamosPorPeriodoAsync(fechaInicio, fechaFin);
            return Ok(prestamos);
        }
    }
}