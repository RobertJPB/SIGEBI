using System;

namespace SIGEBI.Domain.Entities
{
    public class Notificacion
    {
        public Guid Id { get; private set; }
        public Enums.Operacion.TipoNotificacion Tipo { get; private set; }
        public string Mensaje { get; private set; }
        public DateTime Fecha { get; private set; }
        public Enums.Operacion.EstadoNotificacion Estado { get; private set; }
        public Guid UsuarioId { get; private set; }

        private Notificacion() { }

        public Notificacion(
            Guid usuarioId,
            Enums.Operacion.TipoNotificacion tipo,
            string mensaje,
            DateTime fechaUtc)
        {
            if (usuarioId == Guid.Empty)
                throw new ArgumentException("Usuario inválido.", nameof(usuarioId));

            if (string.IsNullOrWhiteSpace(mensaje))
                throw new ArgumentException("El mensaje es obligatorio.", nameof(mensaje));

            Id = Guid.NewGuid();
            UsuarioId = usuarioId;
            Tipo = tipo;
            Mensaje = mensaje.Trim();
            Fecha = fechaUtc;
            Estado = Enums.Operacion.EstadoNotificacion.NoLeida;
        }

        public void MarcarComoLeida()
        {
            if (Estado == Enums.Operacion.EstadoNotificacion.Leida)
                return;

            Estado = Enums.Operacion.EstadoNotificacion.Leida;
        }
    }
}