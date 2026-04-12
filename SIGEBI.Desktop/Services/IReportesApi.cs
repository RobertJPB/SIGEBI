using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using SIGEBI.Business.DTOs;

namespace SIGEBI.Services
{
    public interface IReportesApi
    {
        [Get("/api/Reportes/general")]
        Task<ReporteDTO> GetReporteGeneralAsync();

        [Get("/api/Reportes/prestamos")]
        Task<List<PrestamoResponseDTO>> GetPrestamosPorPeriodoAsync(
            [Query] DateTime fechaInicio, 
            [Query] DateTime fechaFin, 
            [Query] Guid? usuarioId = null, 
            [Query] Guid? recursoId = null);

        [Get("/api/Reportes/historial")]
        Task<List<HistorialReporteDTO>> GetHistorialReportesAsync([Query] int top = 10);

        [Get("/api/Reportes/usuarios-morosos")]
        Task<List<dynamic>> GetUsuariosMasPenalizadosAsync([Query] int top = 5);

        [Get("/api/Reportes/penalizaciones-activas")]
        Task<List<PenalizacionDTO>> GetPenalizacionesActivasAsync();
    }
}
