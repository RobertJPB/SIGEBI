using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGEBI.Domain.Entities
{
    public class Notificacion
    {
        public Guid Id { get; private set; }
        public short Tipo { get; private set; } // 1=Recordatorio, 2=Penalización
        public string Mensaje { get; private set; } = null!;
        public DateTime Fecha { get; private set; }
        public short Estado { get; private set; } // 1=No leída, 2=Leída
        public Guid UsuarioId { get; private set; }

        private Notificacion() { }

        public Notificacion(Guid usuarioId, short tipo, string mensaje)
        {
            Id = Guid.NewGuid();
            UsuarioId = usuarioId;
            Tipo = tipo;
            Mensaje = mensaje;
            Fecha = DateTime.UtcNow;
            Estado = 1;
        }

        public void MarcarComoLeida() => Estado = 2;
    }
}
