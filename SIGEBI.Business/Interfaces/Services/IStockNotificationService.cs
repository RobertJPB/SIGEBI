using System;
using System.Threading.Tasks;

namespace SIGEBI.Business.Interfaces.Services
{
    // interfaz para avisar cuando vuelve a haber stock
    public interface IStockNotificationService
    {
        Task NotificarDisponibilidadAsync(Guid recursoId);
    }
}
