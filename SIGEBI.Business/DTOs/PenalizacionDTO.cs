using System;
namespace SIGEBI.Business.DTOs
{
    public class PenalizacionDTO
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string Motivo { get; set; } = string.Empty;
        public int DiasSancion { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}
