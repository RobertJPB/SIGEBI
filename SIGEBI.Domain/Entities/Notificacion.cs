using System;
using SIGEBI.Domain.Enums.Operacion;

namespace SIGEBI.Domain.Entities
{
    public class Notificacion
    {
        public Guid Id { get; private set; } // ID único
        public Enums.Operacion.TipoNotificacion Tipo { get; private set; } // Tipo (Info, Alerta, etc.)
        public string Mensaje { get; private set; } = null!; // Contenido del aviso
        public DateTime Fecha { get; private set; } // Fecha de emisión
        public Enums.Operacion.EstadoNotificacion Estado { get; private set; } // Estado (Leída/No Leída)
        public Guid UsuarioId { get; private set; } // Usuario destino
        public Usuario Usuario { get; private set; } = null!; // Navegación al usuario

        private Notificacion() { }

        public Notificacion(Guid id, Guid usuarioId, Enums.Operacion.TipoNotificacion tipo, string mensaje, DateTime fechaUtc)
        {
            if (usuarioId == Guid.Empty) throw new ArgumentException("Usuario inválido.", nameof(usuarioId));
            if (string.IsNullOrWhiteSpace(mensaje)) throw new ArgumentException("El mensaje es obligatorio.", nameof(mensaje));

            Id = id;
            UsuarioId = usuarioId;
            Tipo = tipo;
            Mensaje = mensaje.Trim();
            Fecha = fechaUtc;
            
            // Por defecto nacen sin leerse
            Estado = Enums.Operacion.EstadoNotificacion.NoLeida;
        }

        public void MarcarComoLeida()
        {
            if (Estado == Enums.Operacion.EstadoNotificacion.Leida) return;
            Estado = Enums.Operacion.EstadoNotificacion.Leida;
        }
    }
}
