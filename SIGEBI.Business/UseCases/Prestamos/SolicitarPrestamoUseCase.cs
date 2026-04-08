using Microsoft.Extensions.Logging;
using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.Common;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.Interfaces.Services;
using SIGEBI.Business.Interfaces.UseCases.Prestamos;
using SIGEBI.Business.Mappers;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Entities.Recursos;

namespace SIGEBI.Business.UseCases.Prestamos
{
    public class SolicitarPrestamoUseCase : ISolicitarPrestamoUseCase
    {
        private readonly IPrestamoRepository _prestamoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IRecursoRepository _recursoRepository;
        private readonly IPenalizacionRepository _penalizacionRepository;
        private readonly INotificacionRepository _notificacionRepository;
        private readonly IEmailAdapter _emailAdapter;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGuidGenerator _guidGenerator;
        private readonly ILogger<SolicitarPrestamoUseCase> _logger;

        public SolicitarPrestamoUseCase(
            IPrestamoRepository prestamoRepository,
            IUsuarioRepository usuarioRepository,
            IRecursoRepository recursoRepository,
            IPenalizacionRepository penalizacionRepository,
            INotificacionRepository notificacionRepository,
            IEmailAdapter emailAdapter,
            IUnitOfWork unitOfWork,
            IGuidGenerator guidGenerator,
            ILogger<SolicitarPrestamoUseCase> logger)
        {
            _prestamoRepository = prestamoRepository;
            _usuarioRepository = usuarioRepository;
            _recursoRepository = recursoRepository;
            _penalizacionRepository = penalizacionRepository;
            _notificacionRepository = notificacionRepository;
            _emailAdapter = emailAdapter;
            _unitOfWork = unitOfWork;
            _guidGenerator = guidGenerator;
            _logger = logger;
        }

        // Ejecuta el flujo de préstamo: valida usuario/recurso, comprueba disponibilidad y persiste el registro.
        public async Task<PrestamoResponseDTO> EjecutarAsync(Guid usuarioId, Guid recursoId, DateTime? fechaDevolucionEstimada = null)
        {
            var fechaActual = DateTime.UtcNow;

            // 1. Obtención de datos básicos
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId)
                ?? throw new InvalidOperationException("Usuario no encontrado.");

            var recurso = await _recursoRepository.GetByIdAsync(recursoId)
                ?? throw new InvalidOperationException("Recurso no encontrado.");

            var historialUsuario = await _prestamoRepository.GetByUsuarioIdAsync(usuarioId);
            var penalizaciones = await _penalizacionRepository.GetByUsuarioIdAsync(usuarioId);

            try
            {
                // 2. Aplicación Centralizada de Reglas
                PrestamoPolicy.ValidarPrestamo(usuario, recurso, historialUsuario, penalizaciones);

                // 3. Registro de Solicitud Exitosa (Trazabilidad)
                var solicitudExito = SolicitudAcceso.RegistrarExito(_guidGenerator.Create(), usuarioId, recursoId, fechaActual);
                await _unitOfWork.SolicitudesAcceso.AddAsync(solicitudExito);

                // 4. Creación del Préstamo
                int diasPlazo = PrestamoPolicy.ObtenerDiasPlazo(usuario.Rol);
                var prestamo = new Prestamo(_guidGenerator.Create(), usuarioId, recursoId, diasPlazo, fechaActual, fechaDevolucionEstimada);

                recurso.DisminuirStock(); 
                await _prestamoRepository.AddAsync(prestamo);
                _recursoRepository.Update(recurso);

                // 5. Notificación Interna
                var notificacion = NotificacionFactory.CrearNotificacionPrestamo(_guidGenerator.Create(), usuarioId, prestamo.FechaDevolucionEstimada);
                await _notificacionRepository.AddAsync(notificacion);
                
                await _unitOfWork.SaveChangesAsync();

                // 6. Notificaciones Proactivas (Email)
                await EnviarNotificacionesEmailAsync(usuario, recurso, prestamo);

                return PrestamoMapper.ToDTO(prestamo);
            }
            catch (InvalidOperationException ex)
            {
                // REGLA: Toda solicitud de acceso debe quedar registrada, incluso si es rechazada.
                var solicitudRechazo = SolicitudAcceso.RegistrarRechazo(_guidGenerator.Create(), usuarioId, recursoId, fechaActual, ex.Message);
                await _unitOfWork.SolicitudesAcceso.AddAsync(solicitudRechazo);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Solicitud de préstamo rechazada: {Motivo}", ex.Message);
                throw;
            }
        }

        private async Task EnviarNotificacionesEmailAsync(Usuario usuario, RecursoBibliografico recurso, Prestamo prestamo)
        {
            try 
            {
                await _emailAdapter.EnviarAsync(usuario.Correo, "Confirmación de Préstamo - SIGEBI", 
                    $"Hola {usuario.Nombre}, se ha registrado tu préstamo del recurso: {recurso.Titulo}. " +
                    $"Fecha de devolución: {prestamo.FechaDevolucionEstimada:dd/MM/yyyy}.");

                var bibliotecarios = await _usuarioRepository.GetByRolAsync(SIGEBI.Domain.Enums.Seguridad.RolUsuario.Bibliotecario);
                foreach (var biblio in bibliotecarios)
                {
                    await _emailAdapter.EnviarAsync(biblio.Correo, "Nuevo Préstamo Registrado - SIGEBI",
                        $"Se ha registrado un nuevo préstamo por el usuario {usuario.Nombre} (ID: {usuario.Id}). " +
                        $"Recurso: {recurso.Titulo}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error al enviar notificación de email para el préstamo {PrestamoId}.", prestamo.Id);
            }
        }
    }
}
