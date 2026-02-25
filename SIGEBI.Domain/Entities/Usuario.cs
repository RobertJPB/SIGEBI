using System;

namespace SIGEBI.Domain.Entities
{
    public class Usuario
    {
        public Guid Id { get; private set; }
        public string Nombre { get; private set; } = null!;
        public string Correo { get; private set; } = null!;
        public string ContrasenaHash { get; private set; } = null!;
        public Enums.Seguridad.RolUsuario Rol { get; private set; }
        public Enums.Seguridad.EstadoUsuario Estado { get; private set; }

        private Usuario() { }

        public Usuario(string nombre, string correo, string contrasenaHash, Enums.Seguridad.RolUsuario rol)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre es requerido.", nameof(nombre));

            if (string.IsNullOrWhiteSpace(correo) || !correo.Contains("@"))
                throw new ArgumentException("Correo inválido.", nameof(correo));

            if (string.IsNullOrWhiteSpace(contrasenaHash))
                throw new ArgumentException("La contraseña es requerida.", nameof(contrasenaHash));

            Id = Guid.NewGuid();
            Nombre = nombre.Trim();
            Correo = correo.Trim();
            ContrasenaHash = contrasenaHash;
            Rol = rol;
            Estado = Enums.Seguridad.EstadoUsuario.Activo;
        }

        public void Bloquear()
        {
            if (Estado == Enums.Seguridad.EstadoUsuario.Bloqueado)
                return;

            Estado = Enums.Seguridad.EstadoUsuario.Bloqueado;
        }

        public void Activar()
        {
            Estado = Enums.Seguridad.EstadoUsuario.Activo;
        }

        public void Desactivar()
        {
            Estado = Enums.Seguridad.EstadoUsuario.Inactivo;
        }

        public void CambiarRol(Enums.Seguridad.RolUsuario nuevoRol)
        {
            Rol = nuevoRol;
        }

        public void CambiarNombre(string nuevoNombre)
        {
            if (string.IsNullOrWhiteSpace(nuevoNombre))
                throw new ArgumentException("El nombre es requerido.", nameof(nuevoNombre));

            Nombre = nuevoNombre.Trim();
        }

        public void CambiarCorreo(string nuevoCorreo)
        {
            if (string.IsNullOrWhiteSpace(nuevoCorreo) || !nuevoCorreo.Contains("@"))
                throw new ArgumentException("Correo inválido.", nameof(nuevoCorreo));

            Correo = nuevoCorreo.Trim();
        }

        public void CambiarContrasenaHash(string nuevoContrasenaHash)
        {
            if (string.IsNullOrWhiteSpace(nuevoContrasenaHash))
                throw new ArgumentException("La contraseña es requerida.", nameof(nuevoContrasenaHash));

            ContrasenaHash = nuevoContrasenaHash;
        }
    }
}