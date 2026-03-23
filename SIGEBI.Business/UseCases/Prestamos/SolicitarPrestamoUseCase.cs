using Microsoft.Extensions.Logging;
using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Business.Interfaces.Services;
using SIGEBI.Business.Mappers;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.UseCases.Prestamos
{
    public class SolicitarPrestamoUseCase
    {
        private readonly IPrestamoRepository _prestamoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IRecursoRepository _recursoRepository;
        private readonly IPenalizacionRepository _penalizacionRepository;
        private readonly INotificacionRepository _notificacionRepository;
        private readonly IEmailAdapter _emailAdapter;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SolicitarPrestamoUseCase> _logger;

        public SolicitarPrestamoUseCase(
            IPrestamoRepository prestamoRepository,
            IUsuarioRepository usuarioRepository,
            IRecursoRepository recursoRepository,
            IPenalizacionRepository penalizacionRepository,
            INotificacionRepository notificacionRepository,
            IEmailAdapter emailAdapter,
            IUnitOfWork unitOfWork,
            ILogger<SolicitarPrestamoUseCase> logger)
        {
            _prestamoRepository = prestamoRepository;
            _usuarioRepository = usuarioRepository;
            _recursoRepository = recursoRepository;
            _penalizacionRepository = penalizacionRepository;
            _notificacionRepository = notificacionRepository;
            _emailAdapter = emailAdapter;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // Ejecuta el flujo de préstamo: valida usuario/recurso, comprueba disponibilidad y persiste el registro.
        public async Task<PrestamoResponseDTO> EjecutarAsync(Guid usuarioId, Guid recursoId, DateTime? fechaDevolucionEstimada = null)
        {
            // buscamos el usuario primero
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId)
                ?? throw new InvalidOperationException("Usuario no encontrado.");

            // Validamos que exista el recurso (Libro, Revista, etc.)
            var recurso = await _recursoRepository.GetByIdAsync(recursoId)
                ?? throw new InvalidOperationException("Recurso no encontrado.");

            var prestamosActivos = await _prestamoRepository.GetActivosByUsuarioIdAsync(usuarioId);
            var penalizaciones = await _penalizacionRepository.GetByUsuarioIdAsync(usuarioId);


            PrestamoPolicy.ValidarPrestamo(usuario, recurso, prestamosActivos, penalizaciones);

            int diasPlazo = PrestamoPolicy.ObtenerDiasPlazo(usuario);
            var prestamo = new Prestamo(usuarioId, recursoId, diasPlazo, DateTime.UtcNow, fechaDevolucionEstimada);

            recurso.DisminuirStock(); // bajamos el stock
            await _prestamoRepository.AddAsync(prestamo);
            _recursoRepository.Update(recurso);

            // También generamos una notificación persistente en la base de datos (aparte del correo)
            var notificacion = NotificacionFactory.CrearNotificacionPrestamo(usuarioId, prestamo.FechaDevolucionEstimada);
            await _notificacionRepository.AddAsync(notificacion);
            
            await _unitOfWork.SaveChangesAsync();

            // Notificacion proactiva vía email
            try 
            {
                // Notificar al estudiante
                await _emailAdapter.EnviarAsync(usuario.Correo, "Confirmación de Préstamo - SIGEBI", 
                    $"Hola {usuario.Nombre}, se ha registrado tu préstamo del recurso: {recurso.Titulo}. " +
                    $"Fecha de devolución: {prestamo.FechaDevolucionEstimada:dd/MM/yyyy}.");

                // Notificar a todos los bibliotecarios
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

            return PrestamoMapper.ToDTO(prestamo);
        }
    }
}
