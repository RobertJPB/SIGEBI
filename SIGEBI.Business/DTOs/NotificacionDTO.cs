using System;
namespace SIGEBI.Business.DTOs
{
    public class NotificacionDTO
    {
        public Guid Id { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public Guid UsuarioId { get; set; }
    }
}
