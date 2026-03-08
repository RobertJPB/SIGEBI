using SIGEBI.Business.DTOs;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.Mappers
{
    public static class PrestamoMapper
    {
        public static PrestamoResponseDTO ToDTO(Prestamo prestamo)
        {
            return new PrestamoResponseDTO
            {
                Id = prestamo.Id,
                UsuarioId = prestamo.UsuarioId,
                NombreUsuario = prestamo.Usuario?.Nombre ?? string.Empty,
                RecursoId = prestamo.RecursoId,
                TituloRecurso = prestamo.Recurso?.Titulo ?? string.Empty,
                FechaInicio = prestamo.FechaInicio,
                FechaDevolucionEstimada = prestamo.FechaDevolucionEstimada,
                FechaDevolucionReal = prestamo.FechaDevolucionReal,
                Estado = prestamo.EstadoActual.ToString()
            };
        }
    }
}