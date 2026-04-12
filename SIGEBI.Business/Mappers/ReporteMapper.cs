using SIGEBI.Business.DTOs;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.Mappers
{
    public static class ReporteMapper
    {
        public static HistorialReporteDTO ToDTO(Reporte entity)
        {
            return new HistorialReporteDTO
            {
                Id = entity.Id,
                Tipo = entity.Tipo.ToString(),
                FechaGeneracion = entity.FechaGeneracion,
                Parametros = entity.Parametros,
                Resultado = entity.Resultado
            };
        }
    }
}
