using SIGEBI.Business.DTOs;
using SIGEBI.Application.Interfaces;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Business.Mappers;
using SIGEBI.Domain.DomainServices;

namespace SIGEBI.Business.UseCases.Usuarios
{
    public class NotificacionesUseCase
    {
        private readonly INotificacionRepository _notificacionRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUnitOfWork _unitOfWork;

        public NotificacionesUseCase(
            INotificacionRepository notificacionRepository,
            IUsuarioRepository usuarioRepository,
            IUnitOfWork unitOfWork)
        {
            _notificacionRepository = notificacionRepository;
            _usuarioRepository = usuarioRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<NotificacionDTO>> ObtenerPorUsuarioAsync(Guid usuarioId)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId)
                ?? throw new InvalidOperationException("Usuario no encontrado.");

            var notificaciones = await _notificacionRepository.GetByUsuarioIdAsync(usuarioId);
            return notificaciones.Select(NotificacionMapper.ToDTO);
        }

        public async Task MarcarComoLeidaAsync(Guid notificacionId)
        {
            var notificacion = await _notificacionRepository.GetByIdAsync(notificacionId)
                ?? throw new InvalidOperationException("Notificación no encontrada.");

            notificacion.MarcarComoLeida();
            _notificacionRepository.Update(notificacion);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task EnviarNotificacionPrestamoAsync(Guid usuarioId, DateTime fechaDevolucion)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId)
                ?? throw new InvalidOperationException("Usuario no encontrado.");

            var notificacion = NotificacionFactory.CrearNotificacionPrestamo(usuarioId, fechaDevolucion);
            await _notificacionRepository.AddAsync(notificacion);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}