using System;

namespace SIGEBI.Domain.Entities
{
    public class Prestamo
    {
        public Guid Id { get; private set; }
        public Guid UsuarioId { get; private set; }
        public Guid RecursoId { get; private set; }

        public DateTime FechaInicio { get; private set; }
        public DateTime FechaDevolucionEstimada { get; private set; }
        public DateTime? FechaDevolucionReal { get; private set; }

        public Enums.Biblioteca.EstadoPrestamo EstadoActual { get; private set; }

        private Prestamo() { }

        public Prestamo(Guid usuarioId, Guid recursoId, int diasPlazo, DateTime fechaInicioUtc)
        {
            if (usuarioId == Guid.Empty) throw new ArgumentException("UsuarioId inválido.", nameof(usuarioId));
            if (recursoId == Guid.Empty) throw new ArgumentException("RecursoId inválido.", nameof(recursoId));
            if (diasPlazo <= 0) throw new ArgumentException("El plazo en días debe ser mayor que 0.", nameof(diasPlazo));

            Id = Guid.NewGuid();
            UsuarioId = usuarioId;
            RecursoId = recursoId;

            FechaInicio = fechaInicioUtc;
            FechaDevolucionEstimada = fechaInicioUtc.AddDays(diasPlazo);

            EstadoActual = Enums.Biblioteca.EstadoPrestamo.Activo;
        }

        public void Devolver(DateTime fechaDevolucionUtc)
        {
            if (EstadoActual == Enums.Biblioteca.EstadoPrestamo.Devuelto)
                throw new InvalidOperationException("El préstamo ya fue devuelto.");

            if (EstadoActual != Enums.Biblioteca.EstadoPrestamo.Activo &&
                EstadoActual != Enums.Biblioteca.EstadoPrestamo.Atrasado)
                throw new InvalidOperationException($"No se puede devolver un préstamo en estado {EstadoActual}.");

            if (fechaDevolucionUtc < FechaInicio)
                throw new InvalidOperationException("La fecha de devolución no puede ser anterior a la fecha de inicio.");

            FechaDevolucionReal = fechaDevolucionUtc;
            EstadoActual = Enums.Biblioteca.EstadoPrestamo.Devuelto;
        }

        public void MarcarAtrasadoSiAplica(DateTime hoyUtc)
        {
            if (EstadoActual == Enums.Biblioteca.EstadoPrestamo.Devuelto) return;

            if (EstadoActual == Enums.Biblioteca.EstadoPrestamo.Activo &&
                hoyUtc > FechaDevolucionEstimada)
            {
                EstadoActual = Enums.Biblioteca.EstadoPrestamo.Atrasado;
            }
        }
    }
}