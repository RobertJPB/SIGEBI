using System;
using SIGEBI.Domain.Entities.Recursos;

namespace SIGEBI.Domain.Entities
{
    public class Auditoria
    {
        public int Id { get; private set; } // ID autoincremental
        public Guid? UsuarioId { get; private set; } // Usuario que hizo la acción
        public Enums.Auditoria.TipoAccionAuditoria Accion { get; private set; } // Tipo (Insert, Update, etc.)
        public string TablaAfectada { get; private set; } = string.Empty; // Nombre de la tabla
        public string Detalle { get; private set; } = string.Empty; // Descripción del cambio
        public DateTime FechaRegistro { get; private set; } // Fecha del evento
        public string IpAddress { get; private set; } = string.Empty; // IP del cliente

        public Usuario? Usuario { get; private set; }

        private Auditoria() { }

        public Auditoria(
            Guid? usuarioId,
            Enums.Auditoria.TipoAccionAuditoria accion,
            string tablaAfectada,
            string detalle,
            string ipAddress,
            DateTime fechaRegistroUtc)
        {
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
