using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces.UseCases.Usuarios;

namespace SIGEBI.Business.Services
{
    public class GenerarReportesService
    {
        private readonly IGenerarReportesUseCase _generarReportesUseCase;

        public GenerarReportesService(IGenerarReportesUseCase generarReportesUseCase)
        {
            _generarReportesUseCase = generarReportesUseCase;
        }

        // Este service es mas que nada un wrapper para agrupar los reportes
        public async Task<ReporteDTO> GenerarReporteGeneralAsync()
            => await _generarReportesUseCase.GenerarReporteGeneralAsync();

        public async Task<IEnumerable<PrestamoResponseDTO>> ObtenerPrestamosPorPeriodoAsync(
            DateTime fechaInicio, DateTime fechaFin)
            => await _generarReportesUseCase.ObtenerPrestamosPorPeriodoAsync(fechaInicio, fechaFin);

        public async Task<IEnumerable<object>> ObtenerUsuariosMasPenalizadosAsync(int top)
            => await _generarReportesUseCase.ObtenerUsuariosMasPenalizadosAsync(top);

        public async Task<IEnumerable<PenalizacionDTO>> ObtenerPenalizacionesActivasAsync()
            => await _generarReportesUseCase.ObtenerPenalizacionesActivasAsync();
    }
}
