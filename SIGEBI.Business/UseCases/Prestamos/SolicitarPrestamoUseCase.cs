using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.Common;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.Interfaces.Services;
using SIGEBI.Business.Interfaces.UseCases.Prestamos;
using SIGEBI.Business.Mappers;
using SIGEBI.Domain.Interfaces.Services;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Entities.Recursos;
using SIGEBI.Domain.Enums.Auditoria;

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
        private readonly IMemoryCache _cache;
        private readonly IGuidGenerator _guidGenerator;
        private readonly ILogger<SolicitarPrestamoUseCase> _logger;
        private readonly IAuditService _audit;
        private readonly IPrestamoPolicy _prestamoPolicy;

        public SolicitarPrestamoUseCase(
            IPrestamoRepository prestamoRepository,
            IUsuarioRepository usuarioRepository,
            IRecursoRepository recursoRepository,
            IPenalizacionRepository penalizacionRepository,
            INotificacionRepository notificacionRepository,
            IEmailAdapter emailAdapter,
            IUnitOfWork unitOfWork,
            IMemoryCache cache,
            IGuidGenerator guidGenerator,
            ILogger<SolicitarPrestamoUseCase> logger,
            IAuditService audit,
            IPrestamoPolicy prestamoPolicy)
        {
            _prestamoRepository = prestamoRepository;
            _usuarioRepository = usuarioRepository;
            _recursoRepository = recursoRepository;
            _penalizacionRepository = penalizacionRepository;
            _notificacionRepository = notificacionRepository;
            _emailAdapter = emailAdapter;
            _unitOfWork = unitOfWork;
            _cache = cache;
            _guidGenerator = guidGenerator;
            _logger = logger;
            _audit = audit;
            _prestamoPolicy = prestamoPolicy;
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
                _prestamoPolicy.ValidarPrestamo(usuario, recurso, historialUsuario, penalizaciones);


                // 4. Creación del Préstamo
                int diasPlazo = _prestamoPolicy.ObtenerDiasPlazo(usuario.Rol);
                var prestamo = new Prestamo(_guidGenerator.Create(), usuarioId, recursoId, diasPlazo, fechaActual, fechaDevolucionEstimada);

                recurso.DisminuirStock(); 
                await _prestamoRepository.AddAsync(prestamo);
                _recursoRepository.Update(recurso);

                // Auditoría
                await _audit.LogActionAsync(TipoAccionAuditoria.PrestamoRealizado, "Prestamo", 
                    $"Nuevo compromiso de préstamo para el recurso '{recurso.Titulo}' por el usuario {usuario.Nombre}", 
                    usuario.Id);

                // 5. Notificación Interna
                var notificacion = NotificacionFactory.CrearNotificacionPrestamo(_guidGenerator.Create(), usuarioId, prestamo.FechaDevolucionEstimada);
                await _notificacionRepository.AddAsync(notificacion);
                
                await _unitOfWork.SaveChangesAsync();
                
                // Invalidamos la caché del catálogo para que el stock se actualice de inmediato en la web
                _cache.Remove("RecursosDisponibles");
                
                // Obtenemos correos de bibliotecarios ANTES de disparar el segundo plano
                // para evitar problemas con la resolución de alcances (scope) del repositorio.
                var bibliotecarios = await _usuarioRepository.GetByRolAsync(SIGEBI.Domain.Enums.Seguridad.RolUsuario.Bibliotecario);
                var listaCorreosBiblio = bibliotecarios.Select(b => b.Correo.Value).ToList();

                // 6. Notificaciones Proactivas (Email en segundo plano)
                // Usamos _ = Task.Run para no bloquear la respuesta al usuario.
                _ = Task.Run(async () => 
                {
                    await EnviarNotificacionesEmailBackgroundAsync(
                        usuario.Nombre, 
                        usuario.Correo.Value, 
                        recurso.Titulo, 
                        prestamo.FechaDevolucionEstimada, 
                        listaCorreosBiblio);
                });

                return PrestamoMapper.ToDTO(prestamo);
            }
            catch (InvalidOperationException ex)
            {
                // Auditoría de Rechazo
                await _audit.LogActionAsync(TipoAccionAuditoria.AccesoDenegado, "Prestamo", 
                    $"Solicitud de préstamo rechazada para el recurso '{recursoId}': {ex.Message}", 
                    usuarioId);

                _logger.LogInformation("Solicitud de préstamo rechazada: {Motivo}", ex.Message);
                throw;
            }
        }

        private async Task EnviarNotificacionesEmailBackgroundAsync(string nombreUsuario, string correoUsuario, string tituloRecurso, DateTime fechaDevolucion, List<string> correosBibliotecarios)
        {
            try 
            {
                // Notificación al estudiante
                await _emailAdapter.EnviarAsync(correoUsuario, "Confirmación de Préstamo - SIGEBI", 
                    $"Hola {nombreUsuario}, se ha registrado tu préstamo del recurso: {tituloRecurso}. " +
                    $"Fecha de devolución: {fechaDevolucion:dd/MM/yyyy}.");

                // Notificación a bibliotecarios
                foreach (var biblioCorreo in correosBibliotecarios)
                {
                    await _emailAdapter.EnviarAsync(biblioCorreo, "Nuevo Préstamo Registrado - SIGEBI",
                        $"Se ha registrado un nuevo préstamo por el usuario {nombreUsuario} ({correoUsuario}). " +
                        $"Recurso: {tituloRecurso}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error en el proceso de notificación por email en segundo plano.");
            }
        }
    }
}
