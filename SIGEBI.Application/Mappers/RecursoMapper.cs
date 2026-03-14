using SIGEBI.Business.DTOs;
using SIGEBI.Domain.Entities.Recursos;

namespace SIGEBI.Business.Mappers
{
    // Principio SOLID (SRP - Responsabilidad Única):
    // Su trabajo es mapear los Recursos (que pueden ser Libros, Revistas o Documentos) a DTO.
    public static class RecursoMapper
    {
        public static RecursoDetalleDTO ToDTO(RecursoBibliografico recurso)
        {
            var dto = new RecursoDetalleDTO
            {
                Id = recurso.Id,
                Titulo = recurso.Titulo,
                Autor = recurso.Autor,
                Estado = recurso.Estado.ToString(),
                Stock = recurso.Stock,
                TipoRecurso = recurso.GetType().Name,
                CategoriaId = recurso.IdCategoria,
                CategoriaNombre = recurso.Categoria?.Nombre ?? string.Empty,
                ImagenUrl = recurso.ImagenUrl
            };

            if (recurso is Libro libro)
            {
                dto.ISBN = libro.ISBN;
                dto.Editorial = libro.Editorial;
                dto.Anio = libro.Anio;
                dto.Genero = libro.Genero;
            }
            else if (recurso is Revista revista)
            {
                dto.ISSN = revista.ISSN;
                dto.NumeroEdicion = revista.NumeroEdicion;
                dto.FechaPublicacion = revista.FechaPublicacion;
            }
            else if (recurso is Documento documento)
            {
                dto.Formato = documento.Formato;
                dto.Institucion = documento.Institucion;
                dto.Anio = documento.Anio;
            }

            return dto;
        }
    }
}
