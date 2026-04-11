using System;

namespace SIGEBI.Business.DTOs
{
    // ── Requests específicos para procesos de entrada (Capa de Negocio) ──

    public class AgregarLibroRequest
    {
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public int Stock { get; set; }
        public string? Descripcion { get; set; }
        public string? ISBN { get; set; }
        public string? Editorial { get; set; }
        public int? Anio { get; set; }
        public int? NumeroPaginas { get; set; }
        public string? Genero { get; set; }
        public byte[]? ImagenBytes { get; set; }
        public string? ImagenNombre { get; set; }
    }

    public class AgregarRevistaRequest
    {
        public string Titulo { get; set; } = string.Empty;
        public string? Autor { get; set; }
        public int CategoriaId { get; set; }
        public int Stock { get; set; }
        public string? Descripcion { get; set; }
        public int NumeroEdicion { get; set; }
        public string? ISSN { get; set; }
        public int Anio { get; set; }
        public string? Editorial { get; set; }
        public int? NumeroPaginas { get; set; }
        public byte[]? ImagenBytes { get; set; }
        public string? ImagenNombre { get; set; }
    }

    public class AgregarDocumentoRequest
    {
        public string Titulo { get; set; } = string.Empty;
        public string? Autor { get; set; }
        public int CategoriaId { get; set; }
        public int Stock { get; set; }
        public string? Descripcion { get; set; }
        public string? Formato { get; set; }
        public string? Institucion { get; set; }
        public int? Anio { get; set; }
        public int? NumeroPaginas { get; set; }
        public byte[]? ImagenBytes { get; set; }
        public string? ImagenNombre { get; set; }
    }

    public class ActualizarPerfilRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
    }
}
