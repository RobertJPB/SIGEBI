using SIGEBI.Business.DTOs;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.Mappers
{
    public static class PenalizacionMapper
    {
        public static PenalizacionDTO ToDTO(Penalizacion penalizacion)
        {
            int diasPenalizacion = 0;
            if (penalizacion.FechaFin.HasValue)
            {
                diasPenalizacion = (int)(penalizacion.FechaFin.Value - penalizacion.FechaInicio).TotalDays;
            }

            return new PenalizacionDTO
            {
                Id = penalizacion.Id,
                UsuarioId = penalizacion.UsuarioId,
                NombreUsuario = penalizacion.Usuario?.Nombre ?? string.Empty,
                Motivo = penalizacion.Motivo,
                DiasSancion = diasPenalizacion,
                FechaDesde = penalizacion.FechaInicio,
                FechaHasta = penalizacion.FechaFin,
                Estado = penalizacion.Estado.ToString()
            };
        }
    }
}
