using SIGEBI.Business.DTOs;

namespace SIGEBI.Business.Interfaces.UseCases.Usuarios
{
    /// <summary>
    /// Contrato para la generación de reportes estadísticos del sistema.
    /// </summary>
    public interface IGenerarReportesUseCase
    {
        Task<ReporteDTO> GenerarReporteGeneralAsync();
        Task<IEnumerable<PrestamoResponseDTO>> ObtenerPrestamosPorPeriodoAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<object>> ObtenerUsuariosMasPenalizadosAsync(int top = 5);
        Task<IEnumerable<PenalizacionDTO>> ObtenerPenalizacionesActivasAsync();
    }
}
