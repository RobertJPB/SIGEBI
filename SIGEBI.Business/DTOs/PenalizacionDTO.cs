using System;
namespace SIGEBI.Business.DTOs
{
    public class PenalizacionDTO
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string Motivo { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}