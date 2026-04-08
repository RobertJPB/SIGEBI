using SIGEBI.Business.DTOs;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.Mappers
{
    public static class PenalizacionMapper
    {
        public static PenalizacionDTO ToDTO(Penalizacion penalizacion)
        {
            return new PenalizacionDTO
            {
                Id = penalizacion.Id,
                UsuarioId = penalizacion.UsuarioId,
                NombreUsuario = penalizacion.Usuario?.Nombre ?? string.Empty,
                Motivo = penalizacion.Motivo,
                FechaInicio = penalizacion.FechaInicio,
                FechaFin = penalizacion.FechaFin,
                Estado = penalizacion.Estado.ToString()
            };
        }
    }
}
