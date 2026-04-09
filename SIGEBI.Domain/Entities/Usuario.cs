using System;
using System.Collections.Generic;
using SIGEBI.Domain.Enums.Seguridad;

namespace SIGEBI.Domain.Entities
{
    public class Usuario : IDesactivable
    {
        public const string SistemaHashCentinela = "SYSTEM_ACCOUNT_NO_LOGIN";

        public Guid Id { get; private set; }
        public string Nombre { get; private set; } = null!;
        public string Correo { get; private set; } = null!;
        public string ContrasenaHash { get; private set; } = null!;
        public RolUsuario Rol { get; private set; }
        public EstadoUsuario Estado { get; private set; }
        public string? MotivoEstado { get; private set; }

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
            Id = id;
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            Correo = correo ?? throw new ArgumentNullException(nameof(correo));
            ContrasenaHash = contrasenaHash ?? throw new ArgumentNullException(nameof(contrasenaHash));
            Rol = rol;
            Estado = EstadoUsuario.Activo;
        }

        public void Activar() 
        {
            Estado = EstadoUsuario.Activo;
            MotivoEstado = null;
        }

        public void Desactivar(string motivo) 
        {
            if (string.IsNullOrWhiteSpace(motivo)) throw new ArgumentException("El motivo es obligatorio.");
            Estado = EstadoUsuario.Inactivo;
            MotivoEstado = motivo;
        }

        public void Suspender() => Estado = EstadoUsuario.Suspendido;

        public void Bloquear(string motivo)
        {
            if (string.IsNullOrWhiteSpace(motivo)) throw new ArgumentException("El motivo es obligatorio.");
            Estado = EstadoUsuario.Bloqueado;
            MotivoEstado = motivo;
        }

        public void CambiarRol(RolUsuario nuevoRol) => Rol = nuevoRol;
        public void CambiarNombre(string n) => Nombre = n;
        public void CambiarCorreo(string c) => Correo = c;
        public void CambiarContrasenaHash(string h) => ContrasenaHash = h;
        public void ActualizarImagen(string? u) => ImagenUrl = u;
    }
}
