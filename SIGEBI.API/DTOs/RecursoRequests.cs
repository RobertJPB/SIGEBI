using Microsoft.AspNetCore.Http;

namespace SIGEBI.API.DTOs
{
    /// <summary>
    /// Modelo para la creación o edición de un Libro desde la API.
    /// </summary>
    public class AgregarLibroRequest
    {
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public int Stock { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public string Editorial { get; set; } = string.Empty;
        public int Anio { get; set; }
        public string? Genero { get; set; }
        public string? Descripcion { get; set; }
        public IFormFile? Imagen { get; set; }
    }

    /// <summary>
    /// Modelo para la creación o edición de una Revista desde la API.
    /// </summary>
    public class AgregarRevistaRequest
    {
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public int Stock { get; set; }
        public int NumeroEdicion { get; set; }
        public string ISSN { get; set; } = string.Empty;
        public int Anio { get; set; }
        public string? Editorial { get; set; }
        public string? Descripcion { get; set; }
        public IFormFile? Imagen { get; set; }
    }

    /// <summary>
    /// Modelo para la creación o edición de un Documento desde la API.
    /// </summary>
    public class AgregarDocumentoRequest
    {
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public int Stock { get; set; }
        public string Formato { get; set; } = string.Empty;
        public string Institucion { get; set; } = string.Empty;
        public int Anio { get; set; }
        public string? Descripcion { get; set; }
        public IFormFile? Imagen { get; set; }
    }
}
