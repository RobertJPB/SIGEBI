using SIGEBI.Business.DTOs;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.Mappers
{
    public static class NotificacionMapper
    {
        public static NotificacionDTO ToDTO(Notificacion notificacion)
        {
            return new NotificacionDTO
            {
                Id = notificacion.Id,
                Mensaje = notificacion.Mensaje,
                Tipo = notificacion.Tipo.ToString(),
                Estado = notificacion.Estado.ToString(),
                Fecha = notificacion.Fecha,
                UsuarioId = notificacion.UsuarioId
            };
        }
    }
}