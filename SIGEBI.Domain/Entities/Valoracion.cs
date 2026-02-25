using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGEBI.Domain.Entities
{
    public class Valoracion
    {
        public Guid Id { get; private set; }
        public string? Comentario { get; private set; }
        public int Calificacion { get; private set; }
        public Guid UsuarioId { get; private set; }
        public Guid RecursoId { get; private set; }

        private Valoracion() { }

        public Valoracion(Guid usuarioId, Guid recursoId, int calificacion, string? comentario)
        {
            if (calificacion < 1 || calificacion > 5)
                throw new ArgumentOutOfRangeException("La calificación debe estar entre 1 y 5.");

            Id = Guid.NewGuid();
            UsuarioId = usuarioId;
            RecursoId = recursoId;
            Calificacion = calificacion;
            Comentario = comentario;
        }
    }
}
