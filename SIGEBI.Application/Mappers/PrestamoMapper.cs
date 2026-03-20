using SIGEBI.Business.DTOs;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.Mappers
{
    // Su unico trabajo es convertir la entidad Prestamo en el objeto de respuesta DTO para el cliente.
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
                FechaInicio = prestamo.FechaInicio.ToString("yyyy-MM-dd"),
                FechaDevolucionEstimada = prestamo.FechaDevolucionEstimada.ToString("yyyy-MM-dd"),
                FechaDevolucionReal = prestamo.FechaDevolucionReal?.ToString("yyyy-MM-dd"),
                Estado = prestamo.EstadoActual.ToString()
            };
        }
    }
}
