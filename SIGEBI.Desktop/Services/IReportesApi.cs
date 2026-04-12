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
        Task<List<PrestamoResponseDTO>> GetPrestamosPorPeriodoAsync([Query] DateTime fechaInicio, [Query] DateTime fechaFin);

        [Get("/api/Reportes/usuarios-morosos")]
        Task<List<dynamic>> GetUsuariosMasPenalizadosAsync([Query] int top = 5);

        [Get("/api/Reportes/penalizaciones-activas")]
        Task<List<PenalizacionDTO>> GetPenalizacionesActivasAsync();
    }
}
