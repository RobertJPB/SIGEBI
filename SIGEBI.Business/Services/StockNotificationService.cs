using SIGEBI.Business.Interfaces.Common;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.Interfaces.Services;
using SIGEBI.Domain.DomainServices;
using System;
using System.Threading.Tasks;

namespace SIGEBI.Business.Services
{
    // servicio para manejar avisos de stock de forma central
    public class StockNotificationService : IStockNotificationService
    {
        private readonly IListaDeseosRepository _listaDeseosRepository;
        private readonly INotificacionRepository _notificacionRepository;
        private readonly IRecursoRepository _recursoRepository;
        private readonly IGuidGenerator _guidGenerator;

        public StockNotificationService(
            IListaDeseosRepository listaDeseosRepository,
            INotificacionRepository notificacionRepository,
            IRecursoRepository recursoRepository,
            IGuidGenerator guidGenerator)
        {
            _listaDeseosRepository = listaDeseosRepository;
            _notificacionRepository = notificacionRepository;
            _recursoRepository = recursoRepository;
            _guidGenerator = guidGenerator;
        }

        public async Task NotificarDisponibilidadAsync(Guid recursoId)
        {
            // buscamos el recurso para tener el titulo
            var recurso = await _recursoRepository.GetByIdAsync(recursoId);
            if (recurso == null) return;

            // buscamos a los que lo tienen en lista de deseos
            var usuariosInteresados = await _listaDeseosRepository.GetUsuariosInteresadosAsync(recursoId);

            foreach (var usuarioId in usuariosInteresados)
            {
                // crear notificacion usando el factory
                var notificacion = NotificacionFactory.CrearNotificacionDisponibilidad(
                    _guidGenerator.Create(), 
                    usuarioId, 
                    recurso.Titulo);

                await _notificacionRepository.AddAsync(notificacion);
            }
        }
    }
}
