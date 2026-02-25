using System.Threading.Tasks;

namespace SIGEBI.Business.Abstractions.Services
{
    public interface IEmailService
    {
        Task EnviarNotificacionPrestamoAsync(string emailDestino, string mensaje);
        Task EnviarAlertaGenericaAsync(string emailDestino, string asunto, string cuerpo);
    }
}