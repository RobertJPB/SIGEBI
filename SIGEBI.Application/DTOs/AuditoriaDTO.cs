using SIGEBI.Domain.Enums.Auditoria;

namespace SIGEBI.Business.DTOs
{
    public class AuditoriaDTO
    {
        public int Id { get; set; }
        public Guid UsuarioId { get; set; }
        public string NombreUsuario { get; set; } = null!;
        public TipoAccionAuditoria Accion { get; set; }
        public string TablaAfectada { get; set; } = null!;
        public string Detalle { get; set; } = null!;
        public string IpAddress { get; set; } = null!;
        public DateTime FechaRegistro { get; set; }
    }
}