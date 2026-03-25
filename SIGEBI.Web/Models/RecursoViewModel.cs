namespace SIGEBI.Web.Models
{
    public class RecursoViewModel
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public string CategoriaNombre { get; set; } = string.Empty;
        public int Stock { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string TipoRecurso { get; set; } = string.Empty;
        public string? ImagenUrl { get; set; }
        public string? Descripcion { get; set; }
        public string? ISBN { get; set; }
        public string? Editorial { get; set; }
        public int? Anio { get; set; }
        public string? Genero { get; set; }
        public string? ISSN { get; set; }
        public int? NumeroEdicion { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        public string? Formato { get; set; }
        public string? Institucion { get; set; }
        public double PromedioValoraciones { get; set; }
        public List<SIGEBI.Business.DTOs.ValoracionDTO> Valoraciones { get; set; } = new();
    }
}
