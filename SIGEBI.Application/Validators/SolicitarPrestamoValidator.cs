using SIGEBI.Business.DTOs;

namespace SIGEBI.Business.Validators
{
    public class SolicitarPrestamoValidator
    {
        public List<string> Validar(PrestamoRequestDTO dto)
        {
            var errores = new List<string>();

            if (dto.UsuarioId == Guid.Empty)
                errores.Add("El UsuarioId es obligatorio.");

            if (dto.RecursoId == Guid.Empty)
                errores.Add("El RecursoId es obligatorio.");

            return errores;
        }

        public bool EsValido(PrestamoRequestDTO dto)
            => Validar(dto).Count == 0;
    }
}