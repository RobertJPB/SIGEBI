using SIGEBI.Business.DTOs;
using SIGEBI.Domain.Entities.Recursos;

namespace SIGEBI.Business.Mappers
{
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
                TipoRecurso = recurso.GetType().Name
            };

            if (recurso is Libro libro)
            {
                dto.ISBN = libro.ISBN;
                dto.Editorial = libro.Editorial;
                dto.Anio = libro.Anio;
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