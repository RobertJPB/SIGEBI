using System;
using SIGEBI.Domain.Enums.Seguridad;

namespace SIGEBI.Business.Exceptions
{
    /// <summary>
    /// Excepción lanzada cuando un usuario intenta iniciar sesión pero su cuenta
    /// no se encuentra en estado Activo.
    /// </summary>
    public class UsuarioEstadoException : Exception
    {
        public EstadoUsuario Estado { get; }
        public string? Motivo { get; }
        public DateTime? FechaFin { get; }

        public UsuarioEstadoException(EstadoUsuario estado, string? motivo = null, DateTime? fechaFin = null) 
            : base(GetMensajePorEstado(estado, motivo, fechaFin))
        {
            Estado = estado;
            Motivo = motivo;
            FechaFin = fechaFin;
        }

        private static string GetMensajePorEstado(EstadoUsuario estado, string? motivo = null, DateTime? fechaFin = null)
        {
            var msg = estado switch
            {
                EstadoUsuario.Inactivo => "Esta cuenta ha sido desactivada administrativamente.",
                EstadoUsuario.Suspendido => "Su acceso ha sido suspendido temporalmente por infracciones.",
                EstadoUsuario.Bloqueado => "Su cuenta ha sido bloqueada permanentemente por faltas graves.",
                _ => "Su cuenta no tiene permitido el acceso en este momento."
            };

            if (!string.IsNullOrWhiteSpace(motivo))
                msg += $" Razón: {motivo}.";

            if (fechaFin.HasValue)
                msg += $" Fecha estimada de restauración: {fechaFin.Value:dd/MM/yyyy}.";

            if (estado == EstadoUsuario.Inactivo)
                msg += " Contacte al personal de la biblioteca para más información.";

            return msg;
        }
    }
}
