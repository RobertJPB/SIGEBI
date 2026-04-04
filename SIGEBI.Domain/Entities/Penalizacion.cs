using System;

namespace SIGEBI.Domain.Entities
{
    public class Penalizacion
    {
        public Guid Id { get; private set; } // ID único
        public Guid UsuarioId { get; private set; } // ID del sancionado
        public string Motivo { get; private set; } = null!; // Razón de la sanción
        public DateTime FechaInicio { get; private set; } // Fecha de inicio
        public DateTime? FechaFin { get; private set; } // Fecha estimada de fin
        public Enums.Operacion.EstadoPenalizacion Estado { get; private set; } // Estado (Activa, Finalizada)
        public Guid? PrestamoId { get; private set; } // Préstamo asociado
        public Usuario Usuario { get; private set; } = null!; // Navegación al usuario
        public Prestamo? Prestamo { get; private set; } // Navegación al préstamo

        private Penalizacion() { }

        public Penalizacion(Guid id, Guid usuarioId, string motivo, int diasPenalizacion, DateTime fechaInicioUtc, Guid? prestamoId = null)
        {
            if (usuarioId == Guid.Empty) throw new ArgumentException("Usuario inválido.", nameof(usuarioId));
            if (string.IsNullOrWhiteSpace(motivo)) throw new ArgumentException("El motivo es obligatorio.", nameof(motivo));
            if (diasPenalizacion <= 0) throw new ArgumentException("Los días deben ser mayores que 0.", nameof(diasPenalizacion));

            Id = id;
            UsuarioId = usuarioId;
            PrestamoId = prestamoId;
            Motivo = motivo.Trim();
            FechaInicio = fechaInicioUtc;
            
            // Le sumamos los dias de castigo a la fecha actual
            FechaFin = fechaInicioUtc.AddDays(diasPenalizacion);
            
            Estado = Enums.Operacion.EstadoPenalizacion.Activa;
        }

        public void Finalizar(DateTime fechaFinUtc)
        {
            if (Estado == Enums.Operacion.EstadoPenalizacion.Finalizada) return;
            if (fechaFinUtc < FechaInicio)
                throw new InvalidOperationException("La fecha de fin no puede ser anterior a la de inicio.");

            FechaFin = fechaFinUtc;
            Estado = Enums.Operacion.EstadoPenalizacion.Finalizada;
        }
    }
}