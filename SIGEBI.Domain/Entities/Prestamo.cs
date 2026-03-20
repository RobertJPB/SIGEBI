using System;
using SIGEBI.Domain.Entities.Recursos;
using SIGEBI.Domain.Enums.Biblioteca;
using SIGEBI.Domain.DomainServices;

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
        public EstadoPrestamo EstadoActual { get; private set; }
        public Usuario Usuario { get; private set; } = null!;
        public RecursoBibliografico Recurso { get; private set; } = null!;

        private Prestamo() { }

        public Prestamo(Guid usuarioId, Guid recursoId, int diasPlazo, DateTime fechaInicioUtc, DateTime? fechaDevolucionEstimada = null)
        {
            if (usuarioId == Guid.Empty) throw new ArgumentException("UsuarioId inválido.", nameof(usuarioId));
            if (recursoId == Guid.Empty) throw new ArgumentException("RecursoId inválido.", nameof(recursoId));
            
            Id = Guid.NewGuid();
            UsuarioId = usuarioId;
            RecursoId = recursoId;
            FechaInicio = fechaInicioUtc;

            if (fechaDevolucionEstimada.HasValue)
            {
                if (fechaDevolucionEstimada.Value <= fechaInicioUtc)
                    throw new ArgumentException("La fecha de devolución estimada debe ser posterior a la fecha de inicio.", nameof(fechaDevolucionEstimada));
                
                if ((fechaDevolucionEstimada.Value - fechaInicioUtc).TotalDays > PrestamoPolicy.MaxDiasPrestamoTotal)
                    throw new ArgumentException($"La duración del préstamo no puede exceder los {PrestamoPolicy.MaxDiasPrestamoTotal} días.", nameof(fechaDevolucionEstimada));

                FechaDevolucionEstimada = fechaDevolucionEstimada.Value;
            }
            else
            {
                if (diasPlazo <= 0) throw new ArgumentException("El plazo debe ser mayor que 0.", nameof(diasPlazo));
                
                // Aseguramos que el plazo por defecto tampoco exceda el máximo
                int diasEfectivos = Math.Min(diasPlazo, PrestamoPolicy.MaxDiasPrestamoTotal);
                FechaDevolucionEstimada = fechaInicioUtc.AddDays(diasEfectivos);
            }

            EstadoActual = EstadoPrestamo.Activo;
        }

        public void Devolver(DateTime fechaDevolucionUtc)
        {
            if (EstadoActual == EstadoPrestamo.Devuelto)
                throw new InvalidOperationException("El préstamo ya fue devuelto.");
            if (EstadoActual != EstadoPrestamo.Activo && EstadoActual != EstadoPrestamo.Atrasado)
                throw new InvalidOperationException($"No se puede devolver un préstamo en estado {EstadoActual}.");
            if (fechaDevolucionUtc < FechaInicio)
                throw new InvalidOperationException("La fecha de devolución no puede ser anterior a la de inicio.");

            FechaDevolucionReal = fechaDevolucionUtc;
            EstadoActual = EstadoPrestamo.Devuelto;
        }
        
        public void MarcarAtrasadoSiAplica(DateTime hoyUtc)
        {
            // Ojo: no marcar como atrasado si ya lo devolvio!
            if (EstadoActual == EstadoPrestamo.Devuelto) return;
            
            if (EstadoActual == EstadoPrestamo.Activo && hoyUtc > FechaDevolucionEstimada)
                EstadoActual = EstadoPrestamo.Atrasado;
        }
    }
}