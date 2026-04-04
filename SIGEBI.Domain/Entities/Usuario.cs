using System;
using System.Collections.Generic;
using SIGEBI.Domain.Enums.Seguridad;

namespace SIGEBI.Domain.Entities
{
    public class Usuario : IDesactivable
    {
        /// <summary>
        /// Hash centinela que marca la cuenta de sistema. Este valor nunca puede
        /// ser generado por BCrypt, por lo que garantiza que la cuenta no puede
        /// autenticarse a través del flujo normal de login.
        /// </summary>
        public const string SistemaHashCentinela = "SYSTEM_ACCOUNT_NO_LOGIN";

        public Guid Id { get; private set; } // ID único
        public string Nombre { get; private set; } = null!; // Nombre completo
        public string Correo { get; private set; } = null!; // Email institucional
        public string ContrasenaHash { get; private set; } = null!; // Password cifrada
        public RolUsuario Rol { get; private set; } // Nivel de acceso
        public EstadoUsuario Estado { get; private set; } // Estado actual

        public ICollection<Prestamo> Prestamos { get; private set; } = new List<Prestamo>();
        public ICollection<Penalizacion> Penalizaciones { get; private set; } = new List<Penalizacion>();
        public ICollection<Notificacion> Notificaciones { get; private set; } = new List<Notificacion>();
        public ICollection<Valoracion> Valoraciones { get; private set; } = new List<Valoracion>();
        public ICollection<Auditoria> Auditorias { get; private set; } = new List<Auditoria>();
        public ListaDeseos? ListaDeseos { get; private set; }
        public string? ImagenUrl { get; private set; }

        private Usuario() { }

        public Usuario(Guid id, string nombre, string correo, string contrasenaHash, RolUsuario rol)
        {
            // Validaciones basicas antes de crear el usuario
            Id = id;
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre es requerido.", nameof(nombre));
            
            // Validación de correo usando la anotación estándar de .NET
            if (string.IsNullOrWhiteSpace(correo) || !new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(correo))
                throw new ArgumentException("Correo inválido.", nameof(correo));
            if (string.IsNullOrWhiteSpace(contrasenaHash))
                throw new ArgumentException("La contraseña es requerida.", nameof(contrasenaHash));

            Nombre = nombre.Trim();
            Correo = correo.Trim();
            ContrasenaHash = contrasenaHash;
            Rol = rol;
            Estado = EstadoUsuario.Activo;
        }

        public void Bloquear()
        {
            if (Estado == EstadoUsuario.Bloqueado) return;
            Estado = EstadoUsuario.Bloqueado;
        }

        public void Activar() => Estado = EstadoUsuario.Activo;
        public void Desactivar() => Estado = EstadoUsuario.Inactivo;
        public void CambiarRol(RolUsuario nuevoRol) => Rol = nuevoRol;

        public void CambiarNombre(string nuevoNombre)
        {
            if (string.IsNullOrWhiteSpace(nuevoNombre))
                throw new ArgumentException("El nombre es requerido.", nameof(nuevoNombre));
            Nombre = nuevoNombre.Trim();
        }

        public void CambiarCorreo(string nuevoCorreo)
        {
            if (string.IsNullOrWhiteSpace(nuevoCorreo) || !new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(nuevoCorreo))
                throw new ArgumentException("Correo inválido.", nameof(nuevoCorreo));
            Correo = nuevoCorreo.Trim();
        }

        public void CambiarContrasenaHash(string nuevoHash)
        {
            if (string.IsNullOrWhiteSpace(nuevoHash))
                throw new ArgumentException("La contraseña es requerida.", nameof(nuevoHash));
            ContrasenaHash = nuevoHash;
        }

        public void ActualizarImagen(string? nuevaUrl) => ImagenUrl = nuevaUrl;
    }
}
