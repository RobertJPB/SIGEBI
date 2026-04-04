using System;
using SIGEBI.Business.Interfaces.Common;

namespace SIGEBI.Infrastructure.Common
{
    /// <summary>
    /// Generador de IDs secuenciales optimizado para SQL Server.
    /// Reduce la fragmentación de índices al insertar registros.
    /// </summary>
    public class SequentialGuidGenerator : IGuidGenerator
    {
        public Guid Create()
        {
            byte[] guidArray = Guid.NewGuid().ToByteArray();

            DateTime baseDate = new DateTime(1900, 1, 1);
            DateTime now = DateTime.UtcNow;

            TimeSpan days = new TimeSpan(now.Ticks - baseDate.Ticks);
            TimeSpan msecs = now.TimeOfDay;

            byte[] daysArray = BitConverter.GetBytes(days.Days);
            byte[] msecsArray = BitConverter.GetBytes((long)(msecs.TotalMilliseconds / 3.333333)); // Precisión de SQL Server

            Array.Reverse(daysArray);
            Array.Reverse(msecsArray);

            Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
            Array.Copy(msecsArray, msecsArray.Length - 4, guidArray, guidArray.Length - 4, 4);

            return new Guid(guidArray);
        }
    }
}
