using SIGEBI.Business.DTOs;

namespace SIGEBI.Business.Validators
{
    public class ValoracionValidator
    {
        public List<string> Validar(ValoracionDTO dto)
        {
            var errores = new List<string>();

            if (dto.UsuarioId == Guid.Empty)
                errores.Add("El UsuarioId es obligatorio.");

            if (dto.RecursoId == Guid.Empty)
                errores.Add("El RecursoId es obligatorio.");

            if (dto.Calificacion < 1 || dto.Calificacion > 5)
                errores.Add("La calificación debe estar entre 1 y 5.");

            if (dto.Comentario != null && dto.Comentario.Length > 500)
                errores.Add("El comentario no puede superar los 500 caracteres.");

            return errores;
        }

        public bool EsValido(ValoracionDTO dto)
            => Validar(dto).Count == 0;
    }
}
