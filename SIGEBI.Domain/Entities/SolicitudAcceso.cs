using System;
using SIGEBI.Domain.Entities.Recursos;

namespace SIGEBI.Domain.Entities
{
    /// <summary>
    /// Entidad que registra cada solicitud de acceso a un recurso bibliográfico.
    /// Esta entidad es clave para la trazabilidad y auditoría requerida por el reglamento.
    /// </summary>
    public class SolicitudAcceso
    {
        public Guid Id { get; private set; }
        public Guid UsuarioId { get; private set; }
        public Guid RecursoId { get; private set; }
        public DateTime FechaSolicitud { get; private set; }
        public bool FueAprobada { get; private set; }
        public string? MotivoRechazo { get; private set; }

        public Usuario Usuario { get; private set; } = null!;
        public RecursoBibliografico Recurso { get; private set; } = null!;

        private SolicitudAcceso() { }

        public static SolicitudAcceso RegistrarExito(Guid id, Guid usuarioId, Guid recursoId, DateTime fecha)
        {
            return new SolicitudAcceso
            {
                Id = id,
                UsuarioId = usuarioId,
                RecursoId = recursoId,
                FechaSolicitud = fecha,
                FueAprobada = true
            };
        }

        public static SolicitudAcceso RegistrarRechazo(Guid id, Guid usuarioId, Guid recursoId, DateTime fecha, string motivo)
        {
            return new SolicitudAcceso
            {
                Id = id,
                UsuarioId = usuarioId,
                RecursoId = recursoId,
                FechaSolicitud = fecha,
                FueAprobada = false,
                MotivoRechazo = motivo
            };
        }
    }
}
