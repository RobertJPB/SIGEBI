using System;
using System.Collections.Generic;
using SIGEBI.Domain.Enums.Seguridad;

namespace SIGEBI.Domain.Entities
{
    // La entidad Usuario solo es responsable de gestionar sus propios datos y su estado.
    // No sabe como guardarse en la BD ni como mandarse por mail.
    public class Usuario
    {
        public Guid Id { get; private set; }
        public string Nombre { get; private set; } = null!;
        public string Correo { get; private set; } = null!;
        public string ContrasenaHash { get; private set; } = null!;
        public RolUsuario Rol { get; private set; }
        public EstadoUsuario Estado { get; private set; }

        public ICollection<Prestamo> Prestamos { get; private set; } = new List<Prestamo>();
        public ICollection<Penalizacion> Penalizaciones { get; private set; } = new List<Penalizacion>();
        public ICollection<Notificacion> Notificaciones { get; private set; } = new List<Notificacion>();
        public ICollection<Valoracion> Valoraciones { get; private set; } = new List<Valoracion>();
        public ICollection<Auditoria> Auditorias { get; private set; } = new List<Auditoria>();
        public ListaDeseos? ListaDeseos { get; private set; }
        public string? ImagenUrl { get; private set; }

        private Usuario() { }

        public Usuario(string nombre, string correo, string contrasenaHash, RolUsuario rol)
        {
            // Validaciones re basicas antes de crear el usuario
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre es requerido.", nameof(nombre));
            
            // Validación de correo usando la anotación estándar de .NET
            if (string.IsNullOrWhiteSpace(correo) || !new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(correo))
                throw new ArgumentException("Correo inválido.", nameof(correo));
            if (string.IsNullOrWhiteSpace(contrasenaHash))
                throw new ArgumentException("La contraseña es requerida.", nameof(contrasenaHash));

            Id = Guid.NewGuid();
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
