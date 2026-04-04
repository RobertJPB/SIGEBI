using System;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Enums.Operacion;

namespace SIGEBI.Domain.DomainServices
{
    public class NotificacionFactory
    {
        public static Notificacion CrearNotificacionPrestamo(Guid id, Guid usuarioId, DateTime fechaDevolucion)
        {
            var mensaje = $"Tu préstamo vence el {fechaDevolucion:dd/MM/yyyy}. Por favor devuelve el recurso a tiempo.";
            return new Notificacion(id, usuarioId, TipoNotificacion.Recordatorio, mensaje, DateTime.UtcNow);
        }

        public static Notificacion CrearNotificacionDevolucion(Guid id, Guid usuarioId)
        {
            var mensaje = "Has devuelto el recurso exitosamente. Gracias.";
            return new Notificacion(id, usuarioId, TipoNotificacion.Devolucion, mensaje, DateTime.UtcNow);
        }

        public static Notificacion CrearNotificacionPenalizacion(Guid id, Guid usuarioId, string motivo, DateTime fechaFin)
        {
            var mensaje = $"Has recibido una penalización por: {motivo}. Estará activa hasta el {fechaFin:dd/MM/yyyy}.";
            return new Notificacion(id, usuarioId, TipoNotificacion.Penalizacion, mensaje, DateTime.UtcNow);
        }

        public static Notificacion CrearNotificacionAtraso(Guid id, Guid usuarioId)
        {
            var mensaje = "Tienes un préstamo atrasado. Por favor devuelve el recurso lo antes posible.";
            return new Notificacion(id, usuarioId, TipoNotificacion.Atraso, mensaje, DateTime.UtcNow);
        }

        public static Notificacion CrearNotificacionDisponibilidad(Guid id, Guid usuarioId, string tituloRecurso)
        {
            var mensaje = $"¡Buenas noticias! El recurso '{tituloRecurso}' que tienes en tu lista de deseos ya está disponible.";
            return new Notificacion(id, usuarioId, TipoNotificacion.Recordatorio, mensaje, DateTime.UtcNow);
        }
    }
}