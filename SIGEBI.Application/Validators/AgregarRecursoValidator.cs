using SIGEBI.Business.DTOs;

namespace SIGEBI.Business.Validators
{
    public class AgregarRecursoValidator
    {
        public List<string> Validar(RecursoDetalleDTO dto)
        {
            var errores = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Titulo))
                errores.Add("El título es obligatorio.");
            else if (dto.Titulo.Length > 200)
                errores.Add("El título no puede superar los 200 caracteres.");

            if (string.IsNullOrWhiteSpace(dto.Autor))
                errores.Add("El autor es obligatorio.");
            else if (dto.Autor.Length > 150)
                errores.Add("El autor no puede superar los 150 caracteres.");

            if (dto.Stock < 0)
                errores.Add("El stock no puede ser negativo.");

            if (string.IsNullOrWhiteSpace(dto.TipoRecurso))
                errores.Add("El tipo de recurso es obligatorio.");
            else if (dto.TipoRecurso != "Libro" && dto.TipoRecurso != "Revista" && dto.TipoRecurso != "Documento")
                errores.Add("El tipo de recurso debe ser Libro, Revista o Documento.");

            if (dto.TipoRecurso == "Libro")
            {
                if (string.IsNullOrWhiteSpace(dto.ISBN))
                    errores.Add("El ISBN es obligatorio para libros.");
                if (string.IsNullOrWhiteSpace(dto.Editorial))
                    errores.Add("La editorial es obligatoria para libros.");
                if (dto.Anio == null || dto.Anio <= 0)
                    errores.Add("El año es obligatorio para libros.");
            }
            else if (dto.TipoRecurso == "Revista")
            {
                if (string.IsNullOrWhiteSpace(dto.ISSN))
                    errores.Add("El ISSN es obligatorio para revistas.");
                if (dto.NumeroEdicion == null || dto.NumeroEdicion <= 0)
                    errores.Add("El número de edición es obligatorio para revistas.");
            }
            else if (dto.TipoRecurso == "Documento")
            {
                if (string.IsNullOrWhiteSpace(dto.Formato))
                    errores.Add("El formato es obligatorio para documentos.");
                if (string.IsNullOrWhiteSpace(dto.Institucion))
                    errores.Add("La institución es obligatoria para documentos.");
            }

            return errores;
        }

        public bool EsValido(RecursoDetalleDTO dto)
            => Validar(dto).Count == 0;
    }
}