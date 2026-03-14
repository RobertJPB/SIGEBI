using SIGEBI.Business.DTOs;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.Mappers
{
    // Principio SOLID (SRP - Responsabilidad Única):
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
                FechaInicio = prestamo.FechaInicio,
                FechaDevolucionEstimada = prestamo.FechaDevolucionEstimada,
                FechaDevolucionReal = prestamo.FechaDevolucionReal,
                Estado = prestamo.EstadoActual.ToString()
            };
        }
    }
}