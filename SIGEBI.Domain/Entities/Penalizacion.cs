using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGEBI.Domain.Entities
{
    public class Penalizacion
    {
        public Guid Id { get; private set; }
        public string Motivo { get; private set; } = null!;
        public DateTime FechaInicio { get; private set; }
        public DateTime? FechaFin { get; private set; }
        public Guid UsuarioId { get; private set; }
        public short Estado { get; private set; }

        private Penalizacion() { }

        public Penalizacion(Guid usuarioId, string motivo, int diasPenalizacion)
        {
            Id = Guid.NewGuid();
            UsuarioId = usuarioId;
            Motivo = motivo;
            FechaInicio = DateTime.UtcNow;
            FechaFin = FechaInicio.AddDays(diasPenalizacion);
            Estado = 1; // Activa
        }

        public void FinalizarPenalizacion()
        {
            FechaFin = DateTime.UtcNow;
            Estado = 2; // Finalizada
        }
    }
}
