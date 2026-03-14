using SIGEBI.Business.DTOs;

namespace SIGEBI.Business.Validators
{
    // Principio SOLID (SRP - Responsabilidad Única):
    // Solo se encarga de verificar que los datos crudos del usuario sean correctos antes del registro.
    public class RegistrarUsuarioValidator
    {
        public List<string> Validar(UsuarioDTO dto)
        {
            var errores = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                errores.Add("El nombre es obligatorio.");
            else if (dto.Nombre.Length > 100)
                errores.Add("El nombre no puede superar los 100 caracteres.");

            if (string.IsNullOrWhiteSpace(dto.Correo))
                errores.Add("El correo es obligatorio.");
            else if (!dto.Correo.Contains("@"))
                errores.Add("El correo no es válido.");

            if (string.IsNullOrWhiteSpace(dto.Contrasena))
                errores.Add("La contraseña es obligatoria.");
            else if (dto.Contrasena.Length < 6)
                // TODO: Habría que agregar verificacion de mayusculas y numeros
                errores.Add("La contraseña debe tener al menos 6 caracteres.");

            return errores;
        }

        public bool EsValido(UsuarioDTO dto)
            => Validar(dto).Count == 0;
    }
}