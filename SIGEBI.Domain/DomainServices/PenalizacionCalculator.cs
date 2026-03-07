using System;
namespace SIGEBI.Domain.DomainServices
{
    public class PenalizacionCalculator
    {
        private const int DiasPorDiaDeAtraso = 3;
        private const int MaxDiasPenalizacion = 30;

        public static int CalcularDiasPenalizacion(DateTime fechaDevolucionEstimada, DateTime fechaDevolucionReal)
        {
            if (fechaDevolucionReal <= fechaDevolucionEstimada)
                return 0;

            int diasAtraso = (int)(fechaDevolucionReal - fechaDevolucionEstimada).TotalDays;
            int diasPenalizacion = diasAtraso * DiasPorDiaDeAtraso;

            return Math.Min(diasPenalizacion, MaxDiasPenalizacion);
        }

        public static bool TienePenalizacion(DateTime fechaDevolucionEstimada, DateTime fechaDevolucionReal)
        {
            return fechaDevolucionReal > fechaDevolucionEstimada;
        }

        public static string ObtenerMotivo(int diasAtraso)
        {
            return $"Devolución con {diasAtraso} día(s) de atraso.";
        }
    }
}