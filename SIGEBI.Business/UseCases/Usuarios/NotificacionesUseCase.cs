using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.Interfaces.UseCases.Usuarios;
using SIGEBI.Business.Mappers;
using SIGEBI.Business.Interfaces.Common;
using SIGEBI.Domain.DomainServices;

namespace SIGEBI.Business.UseCases.Usuarios
{
    // Centraliza la gestión de alertas y mensajes enviados a los usuarios.
    public class NotificacionesUseCase : INotificacionesUseCase
    {
        private readonly INotificacionRepository _notificacionRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGuidGenerator _guidGenerator;

        public NotificacionesUseCase(
            INotificacionRepository notificacionRepository,
            IUsuarioRepository usuarioRepository,
            IUnitOfWork unitOfWork,
            IGuidGenerator guidGenerator)
        {
            _notificacionRepository = notificacionRepository;
            _usuarioRepository = usuarioRepository;
            _unitOfWork = unitOfWork;
            _guidGenerator = guidGenerator;
        }

        public async Task<IEnumerable<NotificacionDTO>> ObtenerPorUsuarioAsync(Guid usuarioId)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId)
                ?? throw new InvalidOperationException("Usuario no encontrado.");

            var notificaciones = await _notificacionRepository.GetByUsuarioIdAsync(usuarioId);
            return notificaciones.Select(NotificacionMapper.ToDTO);
        }

        public async Task<int> ObtenerCantPendientesAsync(Guid usuarioId)
        {
            return await _notificacionRepository.GetCantPendientesAsync(usuarioId);
        }

        public async Task MarcarComoLeidaAsync(Guid notificacionId)
        {
            var notificacion = await _notificacionRepository.GetByIdAsync(notificacionId)
                ?? throw new KeyNotFoundException("Notificación no encontrada.");

            notificacion.MarcarComoLeida(); // cambiamos el estado
            _notificacionRepository.Update(notificacion);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task EliminarNotificacionAsync(Guid id)
        {
            var notificacion = await _notificacionRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Notificación no encontrada.");

            _notificacionRepository.Delete(notificacion);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<NotificacionDTO> ObtenerPorIdAsync(Guid id)
        {
            var notificacion = await _notificacionRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Notificación no encontrada.");
            return NotificacionMapper.ToDTO(notificacion);
        }

        public async Task EnviarNotificacionPrestamoAsync(Guid usuarioId, DateTime fechaDevolucion)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId)
                ?? throw new InvalidOperationException("Usuario no encontrado.");

            var notificacion = NotificacionFactory.CrearNotificacionPrestamo(_guidGenerator.Create(), usuarioId, fechaDevolucion);
            await _notificacionRepository.AddAsync(notificacion);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}