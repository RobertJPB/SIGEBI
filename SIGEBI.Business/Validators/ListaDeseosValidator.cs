namespace SIGEBI.Business.Validators
{
    public class ListaDeseosValidator
    {
        public List<string> Validar(Guid usuarioId, Guid recursoId)
        {
            var errores = new List<string>();

            if (usuarioId == Guid.Empty)
                errores.Add("El UsuarioId es obligatorio.");

            if (recursoId == Guid.Empty)
                errores.Add("El RecursoId es obligatorio.");

            return errores;
        }

        public bool EsValido(Guid usuarioId, Guid recursoId)
            => Validar(usuarioId, recursoId).Count == 0;
    }
}
