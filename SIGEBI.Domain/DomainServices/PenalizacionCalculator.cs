using System;
namespace SIGEBI.Domain.DomainServices
{
    public class PenalizacionCalculator
    {
        private const int DiasPorDiaDeAtraso = 3;
        private const int MaxDiasPenalizacion = 30;

        public static int CalcularDiasPenalizacion(DateTime fechaDevolucionEstimada, DateTime fechaDevolucionReal)
        {
            // Si lo devolvio a tiempo o antes, no hay multa
            if (fechaDevolucionReal <= fechaDevolucionEstimada)
                return 0;

            // Calculamos cuanto se tardo (en dias enteros)
            int diasAtraso = (int)(fechaDevolucionReal - fechaDevolucionEstimada).TotalDays;
            
            // La multa es: dias de atraso * 3 (ej: 2 dias tarde = 6 dias de castigo)
            int diasPenalizacion = diasAtraso * DiasPorDiaDeAtraso;

            // Retornamos el menor valor entre el calculo y el tope maximo (30 dias)
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
