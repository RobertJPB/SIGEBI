using SIGEBI.Business.DTOs;

namespace SIGEBI.Business.Interfaces.UseCases.Usuarios
{
    public interface IGenerarReportesUseCase
    {
        Task<ReporteDTO> GenerarReporteGeneralAsync();
        Task<IEnumerable<PrestamoResponseDTO>> ObtenerPrestamosPorPeriodoAsync(DateTime fechaInicio, DateTime fechaFin, Guid? usuarioId = null, Guid? recursoId = null);
        Task<IEnumerable<object>> ObtenerUsuariosMasPenalizadosAsync(int top = 5);
        Task<IEnumerable<PenalizacionDTO>> ObtenerPenalizacionesActivasAsync();
        Task<IEnumerable<HistorialReporteDTO>> ObtenerHistorialReportesAsync(int count = 10);
    }
}

