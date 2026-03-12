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

        public async Task<ReporteDTO> GenerarReporteGeneralAsync()
            => await _generarReportesUseCase.GenerarReporteGeneralAsync();

        public async Task<IEnumerable<PrestamoResponseDTO>> ObtenerPrestamosPorPeriodoAsync(
            DateTime fechaInicio, DateTime fechaFin)
            => await _generarReportesUseCase.ObtenerPrestamosPorPeriodoAsync(fechaInicio, fechaFin);
    }
}
