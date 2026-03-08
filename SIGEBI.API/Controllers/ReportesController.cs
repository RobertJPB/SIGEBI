using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.UseCases.Usuarios;

namespace SIGEBI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador,Bibliotecario")]
    public class ReportesController : ControllerBase
    {
        private readonly GenerarReportesUseCase _reportesUseCase;

        public ReportesController(GenerarReportesUseCase reportesUseCase)
        {
            _reportesUseCase = reportesUseCase;
        }

        // Obtener reporte general del sistema
        [HttpGet("general")]
        public async Task<IActionResult> ObtenerReporteGeneral()
        {
            var reporte = await _reportesUseCase.GenerarReporteGeneralAsync();
            return Ok(reporte);
        }

        // Obtener prestamos por periodo
        [HttpGet("prestamos")]
        public async Task<IActionResult> ObtenerPrestamosPorPeriodo(
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin)
        {
            var prestamos = await _reportesUseCase.ObtenerPrestamosPorPeriodoAsync(fechaInicio, fechaFin);
            return Ok(prestamos);
        }
    }
}