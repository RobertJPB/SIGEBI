using SIGEBI.Business.DTOs;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.Mappers
{
    public static class ValoracionMapper
    {
        public static ValoracionDTO ToDTO(Valoracion valoracion)
        {
            return new ValoracionDTO
            {
                Id = valoracion.Id,
                UsuarioId = valoracion.UsuarioId,
                NombreUsuario = valoracion.Usuario?.Nombre ?? string.Empty,
                RecursoId = valoracion.RecursoId,
                Calificacion = valoracion.Calificacion,
                Comentario = valoracion.Comentario,
                ImagenUrl = valoracion.Usuario?.ImagenUrl
            };
        }
    }
}