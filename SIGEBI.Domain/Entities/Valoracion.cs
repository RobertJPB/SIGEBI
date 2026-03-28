using System;
namespace SIGEBI.Domain.Entities;
using SIGEBI.Domain.Entities.Recursos;


public class Valoracion
{
    public Guid Id { get; private set; } // ID único
    public Guid UsuarioId { get; private set; } // ID del autor
    public Guid RecursoId { get; private set; } // ID del recurso valorado
    public int Calificacion { get; private set; } // Puntaje (1-5)
    public string? Comentario { get; private set; } // Reseña opcional

    public Usuario Usuario { get; private set; } = null!; // Navegación al autor
    public RecursoBibliografico Recurso { get; private set; } = null!; // Material valorado

    private Valoracion() { }

    public Valoracion(Guid usuarioId, Guid recursoId, int calificacion, string? comentario)
    {
        if (usuarioId == Guid.Empty) throw new ArgumentException("Usuario inválido.", nameof(usuarioId));
        if (recursoId == Guid.Empty) throw new ArgumentException("Recurso inválido.", nameof(recursoId));
        ValidarCalificacion(calificacion);

            Id = DomainServices.SequentialGuidGenerator.NewGuid();
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
        // Solo aceptamos de 1 a 5 estrellas
        if (calificacion < 1 || calificacion > 5)
            throw new ArgumentOutOfRangeException(nameof(calificacion), "La calificación debe estar entre 1 y 5.");
    }
}