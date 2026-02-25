using System;
using System.Collections.Generic;
using System.Linq;

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

        // Usamos la ruta completa para que no haya duda del tipo de dato
        public Enums.Biblioteca.EstadoPrestamo EstadoActual { get; private set; }

        private Prestamo() { }

        public Prestamo(Guid usuarioId, Guid recursoId, int diasPlazo)
        {
            Id = Guid.NewGuid();
            UsuarioId = usuarioId;
            RecursoId = recursoId;
            FechaInicio = DateTime.UtcNow;
            FechaDevolucionEstimada = DateTime.UtcNow.AddDays(diasPlazo);

            // Asignación con ruta completa
            EstadoActual = Enums.Biblioteca.EstadoPrestamo.Activo;
        }

        public void FinalizarPrestamo()
        {
            FechaDevolucionReal = DateTime.UtcNow;
            EstadoActual = Enums.Biblioteca.EstadoPrestamo.Devuelto;
        }

        public void VerificarRetraso()
        {
            if (DateTime.UtcNow > FechaDevolucionEstimada && EstadoActual == Enums.Biblioteca.EstadoPrestamo.Activo)
            {
                EstadoActual = Enums.Biblioteca.EstadoPrestamo.Atrasado;
            }
        }
    }
}