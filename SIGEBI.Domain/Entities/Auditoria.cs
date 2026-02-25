using System;

namespace SIGEBI.Domain.Entities
{
    public class Auditoria
    {
        public int Id { get; private set; }
        public Guid UsuarioId { get; private set; }

        public Enums.Auditoria.TipoAccionAuditoria Accion { get; private set; }

        public string TablaAfectada { get; private set; } = string.Empty;
        public string Detalle { get; private set; } = string.Empty;
        public DateTime FechaRegistro { get; private set; }
        public string IpAddress { get; private set; } = string.Empty;

        private Auditoria() { }

        public Auditoria(
            Guid usuarioId,
            Enums.Auditoria.TipoAccionAuditoria accion,
            string tablaAfectada,
            string detalle,
            string ipAddress,
            DateTime fechaRegistroUtc)
        {
            if (usuarioId == Guid.Empty)
                throw new ArgumentException("Usuario inválido.", nameof(usuarioId));

            if (string.IsNullOrWhiteSpace(tablaAfectada))
                throw new ArgumentException("Tabla afectada requerida.", nameof(tablaAfectada));

            if (string.IsNullOrWhiteSpace(ipAddress))
                throw new ArgumentException("IP requerida.", nameof(ipAddress));

            UsuarioId = usuarioId;
            Accion = accion;
            TablaAfectada = tablaAfectada.Trim();
            Detalle = detalle?.Trim() ?? string.Empty;
            IpAddress = ipAddress.Trim();
            FechaRegistro = fechaRegistroUtc;
        }
    }
}