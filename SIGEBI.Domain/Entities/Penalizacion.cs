using System;

namespace SIGEBI.Domain.Entities
{
    public class Penalizacion
    {
        public Guid Id { get; private set; }
        public Guid UsuarioId { get; private set; }
        public string Motivo { get; private set; }
        public DateTime FechaInicio { get; private set; }
        public DateTime? FechaFin { get; private set; }
        public Enums.Operacion.EstadoPenalizacion Estado { get; private set; }

        private Penalizacion() { }

        public Penalizacion(Guid usuarioId, string motivo, int diasPenalizacion, DateTime fechaInicioUtc)
        {
            if (usuarioId == Guid.Empty)
                throw new ArgumentException("Usuario inválido.", nameof(usuarioId));

            if (string.IsNullOrWhiteSpace(motivo))
                throw new ArgumentException("El motivo es obligatorio.", nameof(motivo));

            if (diasPenalizacion <= 0)
                throw new ArgumentException("Los días de penalización deben ser mayores que 0.", nameof(diasPenalizacion));

            Id = Guid.NewGuid();
            UsuarioId = usuarioId;
            Motivo = motivo.Trim();
            FechaInicio = fechaInicioUtc;
            FechaFin = fechaInicioUtc.AddDays(diasPenalizacion);
            Estado = Enums.Operacion.EstadoPenalizacion.Activa;
        }

        public void Finalizar(DateTime fechaFinUtc)
        {
            if (Estado == Enums.Operacion.EstadoPenalizacion.Finalizada)
                return;

            if (fechaFinUtc < FechaInicio)
                throw new InvalidOperationException("La fecha de fin no puede ser anterior a la de inicio.");

            FechaFin = fechaFinUtc;
            Estado = Enums.Operacion.EstadoPenalizacion.Finalizada;
        }
    }
}