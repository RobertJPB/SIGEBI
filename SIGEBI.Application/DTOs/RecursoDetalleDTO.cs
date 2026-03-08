using System;
namespace SIGEBI.Business.DTOs
{
    public class RecursoDetalleDTO
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public int Stock { get; set; }
        public string TipoRecurso { get; set; } = string.Empty;
        public double PromedioValoraciones { get; set; }

        // Libro
        public string? ISBN { get; set; }
        public string? Editorial { get; set; }
        public int? Anio { get; set; }

        // Revista
        public string? ISSN { get; set; }
        public int? NumeroEdicion { get; set; }
        public DateTime? FechaPublicacion { get; set; }

        // Documento
        public string? Formato { get; set; }
        public string? Institucion { get; set; }
    }
}