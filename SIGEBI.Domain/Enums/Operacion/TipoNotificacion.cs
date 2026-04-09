using System;

namespace SIGEBI.Domain.Enums.Operacion
{
    public enum TipoNotificacion
    {
        Informativa = 0,
        Recordatorio = 1,
        Penalizacion = 2,
        Devolucion = 3,
        Atraso = 4
    }
    public enum EstadoNotificacion
    {
        NoLeida = 1,
        Leida = 2
    }
}
