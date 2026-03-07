using System;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Enums.Operacion;

namespace SIGEBI.Domain.DomainServices
{
    public class NotificacionFactory
    {
        public static Notificacion CrearNotificacionPrestamo(Guid usuarioId, DateTime fechaDevolucion)
        {
            var mensaje = $"Tu préstamo vence el {fechaDevolucion:dd/MM/yyyy}. Por favor devuelve el recurso a tiempo.";
            return new Notificacion(usuarioId, TipoNotificacion.Recordatorio, mensaje, DateTime.UtcNow);
        }

        public static Notificacion CrearNotificacionDevolucion(Guid usuarioId)
        {
            var mensaje = "Has devuelto el recurso exitosamente. Gracias.";
            return new Notificacion(usuarioId, TipoNotificacion.Devolucion, mensaje, DateTime.UtcNow);
        }

        public static Notificacion CrearNotificacionPenalizacion(Guid usuarioId, string motivo, DateTime fechaFin)
        {
            var mensaje = $"Has recibido una penalización por: {motivo}. Estará activa hasta el {fechaFin:dd/MM/yyyy}.";
            return new Notificacion(usuarioId, TipoNotificacion.Penalizacion, mensaje, DateTime.UtcNow);
        }

        public static Notificacion CrearNotificacionAtraso(Guid usuarioId)
        {
            var mensaje = "Tienes un préstamo atrasado. Por favor devuelve el recurso lo antes posible.";
            return new Notificacion(usuarioId, TipoNotificacion.Atraso, mensaje, DateTime.UtcNow);
        }
    }
}