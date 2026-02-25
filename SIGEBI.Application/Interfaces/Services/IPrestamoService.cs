using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIGEBI.Business.Abstractions.Services
{
    public interface IPrestamoService
    {
        Task RegistrarPrestamoAsync(int usuarioId, Guid recursoId);
     
        Task RegistrarDevolucionAsync(Guid prestamoId);
        Task<IEnumerable<object>> ObtenerPrestamosActivosPorUsuarioAsync(int usuarioId);
    }
}