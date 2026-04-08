using System;
namespace SIGEBI.Business.DTOs
{
    public class ValoracionDTO
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public Guid RecursoId { get; set; }
        public int Calificacion { get; set; }
        public string? Comentario { get; set; }
        public string? ImagenUrl { get; set; }
    }
}
