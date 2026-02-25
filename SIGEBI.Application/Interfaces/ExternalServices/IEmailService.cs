using System.Threading.Tasks;

namespace SIGEBI.Business.Interfaces.ExternalServices
{
    public interface IEmailService
    {
        Task EnviarNotificacionPrestamoAsync(string emailDestino, string mensaje);
        Task EnviarAlertaGenericaAsync(string emailDestino, string asunto, string cuerpo);
    }
}