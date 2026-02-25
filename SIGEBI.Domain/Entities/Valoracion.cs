using System;

namespace SIGEBI.Domain.Entities
{
    public class Valoracion
    {
        public Guid Id { get; private set; }
        public Guid UsuarioId { get; private set; }
        public Guid RecursoId { get; private set; }
        public int Calificacion { get; private set; }
        public string? Comentario { get; private set; }

        private Valoracion() { }

        public Valoracion(Guid usuarioId, Guid recursoId, int calificacion, string? comentario)
        {
            if (usuarioId == Guid.Empty)
                throw new ArgumentException("Usuario inválido.", nameof(usuarioId));

            if (recursoId == Guid.Empty)
                throw new ArgumentException("Recurso inválido.", nameof(recursoId));

            ValidarCalificacion(calificacion);

            Id = Guid.NewGuid();
            UsuarioId = usuarioId;
            RecursoId = recursoId;
            Calificacion = calificacion;
            Comentario = string.IsNullOrWhiteSpace(comentario) ? null : comentario.Trim();
        }

        public void ActualizarCalificacion(int nuevaCalificacion)
        {
            ValidarCalificacion(nuevaCalificacion);
            Calificacion = nuevaCalificacion;
        }

        public void ActualizarComentario(string? nuevoComentario)
        {
            Comentario = string.IsNullOrWhiteSpace(nuevoComentario) ? null : nuevoComentario.Trim();
        }

        private static void ValidarCalificacion(int calificacion)
        {
            if (calificacion < 1 || calificacion > 5)
                throw new ArgumentOutOfRangeException(nameof(calificacion), "La calificación debe estar entre 1 y 5.");
        }
    }
}