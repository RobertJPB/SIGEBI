using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.Interfaces.UseCases.Usuarios;
using SIGEBI.Business.Mappers;
using SIGEBI.Business.Interfaces.Common;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Enums.Auditoria;
using Microsoft.Extensions.Caching.Memory;

namespace SIGEBI.Business.UseCases.Usuarios
{
    // Identifica devoluciones tardías y aplica sanciones automáticas a los usuarios.
    public class PenalizacionesUseCase : IPenalizacionesUseCase
    {
        private readonly IPrestamoRepository _prestamoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPenalizacionRepository _penalizacionRepository;
        private readonly INotificacionesUseCase _notificaciones;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGuidGenerator _guidGenerator;
        private readonly ILogger<PenalizacionesUseCase> _logger;
        private readonly IMemoryCache _cache;
        private readonly IAuditService _audit;
        private const string CachePrefix = "UserStatus_";

        public PenalizacionesUseCase(
            IPenalizacionRepository penalizacionRepository,
            IUsuarioRepository usuarioRepository,
            IPrestamoRepository prestamoRepository,
            INotificacionesUseCase notificaciones,
            IUnitOfWork unitOfWork,
            IAuditService audit,
            IGuidGenerator guidGenerator,
            ILogger<PenalizacionesUseCase> logger,
            IMemoryCache cache)
        {
            _penalizacionRepository = penalizacionRepository;
            _usuarioRepository = usuarioRepository;
            _prestamoRepository = prestamoRepository;
            _notificaciones = notificaciones;
            _unitOfWork = unitOfWork;
            _audit = audit;
            _guidGenerator = guidGenerator;
            _logger = logger;
            _cache = cache;
        }

        // Proceso por lotes que evalúa préstamos atrasados y genera las penalizaciones correspondientes.
        public async Task AplicarPenalizacionesAsync()
        {
            // Este metodo deberia correrse todos los dias con un cronjob o algo asi
            var prestamosAtrasados = await _prestamoRepository.GetAtrasadosAsync();
            var usuariosPenalizadosEnLote = new HashSet<Guid>();

            foreach (var prestamo in prestamosAtrasados)
            {
                // Comprobamos si ya existe una penalización activa para este préstamo
                // evitando duplicar sanciones si el job corre mas de una vez
                var penalizacionExistente = await _penalizacionRepository.GetByPrestamoIdAsync(prestamo.Id);
                if (penalizacionExistente != null)
                {
                    _logger.LogInformation("Penalización ya existente para PrestamoId {PrestamoId}, omitiendo.", prestamo.Id);
                    continue;
                }

                // Asegurar que no se apliquen múltiples penalizaciones activas al mismo tiempo
                if (usuariosPenalizadosEnLote.Contains(prestamo.UsuarioId))
                {
                    continue;
                }

                var penalizacionesPrevias = await _penalizacionRepository.GetByUsuarioIdAsync(prestamo.UsuarioId);
                if (penalizacionesPrevias.Any(p => p.Estado == SIGEBI.Domain.Enums.Operacion.EstadoPenalizacion.Activa))
                {
                    continue;
                }

                int diasAtraso = (int)(DateTime.UtcNow - prestamo.FechaDevolucionEstimada).TotalDays;
                int diasPenalizacion = PenalizacionCalculator.CalcularDiasPenalizacion(
                    prestamo.FechaDevolucionEstimada, DateTime.UtcNow);
                string motivo = PenalizacionCalculator.ObtenerMotivo(diasAtraso);

                var penalizacion = new Penalizacion(_guidGenerator.Create(), prestamo.UsuarioId, motivo, diasPenalizacion, DateTime.UtcNow, prestamo.Id);
                await _penalizacionRepository.AddAsync(penalizacion);
                usuariosPenalizadosEnLote.Add(prestamo.UsuarioId);
                
                // Sincronizar estado del usuario
                var usuario = await _usuarioRepository.GetByIdAsync(prestamo.UsuarioId);
                if (usuario != null)
                {
                    usuario.Suspender($"Penalización automática por {diasPenalizacion} días. Motivo: {motivo}");
                    _usuarioRepository.Update(usuario);
                    _cache.Remove($"{CachePrefix}{usuario.Id}");

                    // Notificar al usuario sobre su nueva sanción
                    await _notificaciones.EnviarNotificacionAsync(usuario.Id, 
                        $"Su cuenta ha sido suspendida automáticamente por {diasPenalizacion} días. Motivo: {motivo}");

                    await _audit.LogActionAsync(TipoAccionAuditoria.PenalizacionAplicada, "Penalizacion", 
                        $"Aplicada sanción automática a {usuario.Nombre} por {diasPenalizacion} días. Razón: {motivo}");
                }

                _logger.LogInformation("Penalización aplicada al usuario {UsuarioId} por préstamo {PrestamoId} ({Dias} días de atraso).",
                    prestamo.UsuarioId, prestamo.Id, diasAtraso);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AplicarPenalizacionManualAsync(AplicarPenalizacionManualDTO dto)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(dto.UsuarioId)
                ?? throw new KeyNotFoundException("Usuario no encontrado.");

            var penalizacionesExistentes = await _penalizacionRepository.GetByUsuarioIdAsync(usuario.Id);
            if (penalizacionesExistentes.Any(p => p.Estado == SIGEBI.Domain.Enums.Operacion.EstadoPenalizacion.Activa))
            {
                throw new InvalidOperationException("El usuario ya tiene una penalización activa. No se pueden superponer sanciones.");
            }

            var penalizacion = new Penalizacion(_guidGenerator.Create(), dto.UsuarioId, dto.Motivo, dto.DiasPenalizacion, DateTime.UtcNow, dto.PrestamoId);
            await _penalizacionRepository.AddAsync(penalizacion);
            
            // Sincronizar estado del usuario
            usuario.Suspender($"Penalización manual por {dto.DiasPenalizacion} días. Motivo: {dto.Motivo}");
            _usuarioRepository.Update(usuario);
            _cache.Remove($"{CachePrefix}{usuario.Id}");

            // Notificar al usuario
            await _notificaciones.EnviarNotificacionAsync(dto.UsuarioId, 
                $"Se le ha aplicado una sanción manual de {dto.DiasPenalizacion} días. Motivo: {dto.Motivo}");

            await _audit.LogActionAsync(TipoAccionAuditoria.PenalizacionAplicada, "Penalizacion", 
                $"Aplicada sanción manual a {usuario.Nombre} por {dto.DiasPenalizacion} días. Razón: {dto.Motivo}");

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<PenalizacionDTO>> ObtenerTodasLasPenalizacionesAsync()
        {
            var penalizaciones = await _penalizacionRepository.GetAllAsync();
            return penalizaciones.Select(PenalizacionMapper.ToDTO);
        }

        public async Task<IEnumerable<PenalizacionDTO>> ObtenerPenalizacionesPorUsuarioAsync(Guid usuarioId)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId)
                ?? throw new KeyNotFoundException("Usuario no encontrado.");

            var penalizaciones = await _penalizacionRepository.GetByUsuarioIdAsync(usuarioId);
            return penalizaciones.Select(PenalizacionMapper.ToDTO);
        }

        // Elimina permanentemente una sanción del sistema (hard delete).
        // Debe ser usado solo para corregir errores administrativos.
        public async Task EliminarPenalizacionAsync(Guid id)
        {
            var penalizacion = await _penalizacionRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("La penalización no existe.");

            _penalizacionRepository.Delete(penalizacion);
            await _unitOfWork.SaveChangesAsync();
        }

        // Resuelve manualmente una sanción antes de que expire su tiempo natural.
        // Útil si el usuario paga una multa o existe una justificación válida.
        public async Task FinalizarPenalizacionAsync(Guid id)
        {
            var penalizacion = await _penalizacionRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("La penalización no existe.");

            penalizacion.Finalizar(DateTime.UtcNow);
            _penalizacionRepository.Update(penalizacion);

            // Verificar si el usuario tiene otras penalizaciones activas
            var usuario = await _usuarioRepository.GetByIdAsync(penalizacion.UsuarioId);
            if (usuario != null)
            {
                var activas = await _penalizacionRepository.GetByUsuarioIdAsync(usuario.Id);
                if (!activas.Any(p => p.Estado == SIGEBI.Domain.Enums.Operacion.EstadoPenalizacion.Activa && p.Id != id))
                {
                    usuario.Activar();
                    _usuarioRepository.Update(usuario);
                    _cache.Remove($"{CachePrefix}{usuario.Id}");
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
