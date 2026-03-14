using SIGEBI.Business.DTOs;
using SIGEBI.Business.UseCases.Usuarios;

namespace SIGEBI.Business.Services
{
    public class GenerarReportesService
    {
        private readonly GenerarReportesUseCase _generarReportesUseCase;

        public GenerarReportesService(GenerarReportesUseCase generarReportesUseCase)
        {
            _generarReportesUseCase = generarReportesUseCase;
        }

        // Este service es mas que nada un wrapper para agrupar los reportes
        public async Task<ReporteDTO> GenerarReporteGeneralAsync()
            => await _generarReportesUseCase.GenerarReporteGeneralAsync();

        public async Task<IEnumerable<PrestamoResponseDTO>> ObtenerPrestamosPorPeriodoAsync(
            DateTime fechaInicio, DateTime fechaFin)
            => await _generarReportesUseCase.ObtenerPrestamosPorPeriodoAsync(fechaInicio, fechaFin);
    }
}
