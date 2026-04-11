using System.IO;

namespace SIGEBI.Business.DTOs
{
    /// <summary>
    /// Base para las solicitudes de creación/edición de recursos.
    /// </summary>
    public abstract class RecursoBaseRequest
    {
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public int Stock { get; set; }
        public string? Descripcion { get; set; }
        public Stream? ImagenStream { get; set; }
        public string? ImagenNombre { get; set; }
        public int? NumeroPaginas { get; set; }
    }

    public class LibroRequestDTO : RecursoBaseRequest
    {
        public string ISBN { get; set; } = string.Empty;
        public string Editorial { get; set; } = string.Empty;
        public int Anio { get; set; }
        public string? Genero { get; set; }
    }

    public class RevistaRequestDTO : RecursoBaseRequest
    {
        public int NumeroEdicion { get; set; }
        public string ISSN { get; set; } = string.Empty;
        public int Anio { get; set; }
        public string? Editorial { get; set; }
    }

    public class DocumentoRequestDTO : RecursoBaseRequest
    {
        public string Formato { get; set; } = string.Empty;
        public string Institucion { get; set; } = string.Empty;
        public int Anio { get; set; }
    }
}
