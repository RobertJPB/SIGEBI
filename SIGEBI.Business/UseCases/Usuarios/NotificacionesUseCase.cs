using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.Interfaces.UseCases.Usuarios;
using SIGEBI.Business.Mappers;
using SIGEBI.Business.Interfaces.Common;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Domain.Entities;

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

        public async Task MarcarTodasComoLeidasAsync(Guid usuarioId)
        {
            var pends = (await _notificacionRepository.GetByUsuarioIdAsync(usuarioId))
                        .Where(n => n.Estado == SIGEBI.Domain.Enums.Operacion.EstadoNotificacion.NoLeida);

            // Nota: Para mejorar el rendimiento, podríamos usar un método masivo en el repo
            // Pero por ahora lo hacemos individualmente para disparar eventos si fuera necesario
            foreach (var notif in pends)
            {
                // El repositorio asNoTracking nos da entidades desconectadas. 
                // Necesitamos traer la entidad con seguimiento para actualizarla si queremos usar el cambio de estado.
                // Sin embargo, NotificacionRepository.GetByUsuarioIdAsync usa AsNoTracking.
                // Así que traemos la entidad completa por ID.
                var entidad = await _notificacionRepository.GetByIdAsync(notif.Id);
                if (entidad != null)
                {
                    entidad.MarcarComoLeida();
                    _notificacionRepository.Update(entidad);
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task EnviarNotificacionPrestamoAsync(Guid usuarioId, DateTime fechaDevolucion)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId)
                ?? throw new InvalidOperationException("Usuario no encontrado.");

            var notificacion = NotificacionFactory.CrearNotificacionPrestamo(_guidGenerator.Create(), usuarioId, fechaDevolucion);
            await _notificacionRepository.AddAsync(notificacion);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<NotificacionDTO>> ObtenerTodasAsync()
        {
            var notificaciones = await _notificacionRepository.GetAllAsync();
            return notificaciones.Select(NotificacionMapper.ToDTO);
        }

        /// <summary>
        /// Método de conveniencia para enviar notificaciones genéricas (pueden ser sanciones, bloqueos, etc.)
        /// </summary>
        public async Task EnviarNotificacionAsync(Guid usuarioId, string mensaje)
        {
            var notificacion = new Notificacion(
                id: _guidGenerator.Create(),
                usuarioId: usuarioId,
                tipo: SIGEBI.Domain.Enums.Operacion.TipoNotificacion.Informativa,
                mensaje: mensaje,
                fechaUtc: DateTime.UtcNow
            );
            
            await _notificacionRepository.AddAsync(notificacion);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
