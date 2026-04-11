using System;

namespace SIGEBI.Business.DTOs
{
    public class RecursoDetalleDTO
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public string CategoriaNombre { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public int Stock { get; set; }
        public string TipoRecurso { get; set; } = string.Empty;
        public string? ImagenUrl { get; set; }
        public string? Descripcion { get; set; }
        public int? NumeroPaginas { get; set; }
        public string? UsuarioCreadorNombre { get; set; }
        public DateTime FechaCreacion { get; set; }
        public double PromedioValoraciones { get; set; }
        public double? PromedioValoracionesNullable => PromedioValoraciones; // Alias para Desktop
        // Libro
        public string? ISBN { get; set; }
        public string? Editorial { get; set; }
        public int? Anio { get; set; }
        public string? Genero { get; set; }
        // Revista
        public string? ISSN { get; set; }
        public int? NumeroEdicion { get; set; }
        // Documento
        public string? Formato { get; set; }
        public string? Institucion { get; set; }
    }
}
