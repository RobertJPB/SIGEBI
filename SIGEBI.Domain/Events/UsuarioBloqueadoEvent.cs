using System;
using SIGEBI.Domain.Common;

namespace SIGEBI.Domain.Events
{
    public class UsuarioBloqueadoEvent : IDomainEvent
    {
        public Guid UsuarioId { get; }
        public string Nombre { get; }
        public string Correo { get; }
        public string Motivo { get; }
        public DateTime OccurredOn { get; }

        public UsuarioBloqueadoEvent(Guid usuarioId, string nombre, string correo, string motivo)
        {
            UsuarioId = usuarioId;
            Nombre = nombre;
            Correo = correo;
            Motivo = motivo;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
