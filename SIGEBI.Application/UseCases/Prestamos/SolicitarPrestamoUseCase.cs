using SIGEBI.Business.DTOs;
using SIGEBI.Application.Interfaces;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Business.Interfaces.Services;
using SIGEBI.Business.Mappers;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.UseCases.Prestamos
{
    // Esta clase NO conoce a SQL Server ni a Entity Framework. 
    // Solo conoce las interfaces (IPrestamoRepository, etc) que le inyectaron por constructor.
    // Gestiona la solicitud de nuevos préstamos validando reglas de negocio y enviando notificaciones.
    public class SolicitarPrestamoUseCase
    {
        private readonly IPrestamoRepository _prestamoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IRecursoRepository _recursoRepository;
        private readonly IPenalizacionRepository _penalizacionRepository;
        private readonly IEmailAdapter _emailAdapter;
        private readonly IUnitOfWork _unitOfWork;

        public SolicitarPrestamoUseCase(
            IPrestamoRepository prestamoRepository,
            IUsuarioRepository usuarioRepository,
            IRecursoRepository recursoRepository,
            IPenalizacionRepository penalizacionRepository,
            IEmailAdapter emailAdapter,
            IUnitOfWork unitOfWork)
        {
            _prestamoRepository = prestamoRepository;
            _usuarioRepository = usuarioRepository;
            _recursoRepository = recursoRepository;
            _penalizacionRepository = penalizacionRepository;
            _emailAdapter = emailAdapter;
            _unitOfWork = unitOfWork;
        }

        // Ejecuta el flujo de préstamo: valida usuario/recurso, comprueba disponibilidad y persiste el registro.
        public async Task<PrestamoResponseDTO> EjecutarAsync(Guid usuarioId, Guid recursoId)
        {
            // buscamos el usuario primero
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId)
                ?? throw new InvalidOperationException("Usuario no encontrado.");

            var recurso = await _recursoRepository.GetByIdAsync(recursoId)
                ?? throw new InvalidOperationException("Recurso no encontrado.");

            var prestamosActivos = await _prestamoRepository.GetActivosByUsuarioIdAsync(usuarioId);
            var penalizaciones = await _penalizacionRepository.GetByUsuarioIdAsync(usuarioId);

            // TODO: Preguntar al profe si esta validacion deberia ir en el dominio o aca
            PrestamoPolicy.ValidarPrestamo(usuario, recurso, prestamosActivos, penalizaciones);

            int diasPlazo = PrestamoPolicy.ObtenerDiasPlazo(usuario);
            var prestamo = new Prestamo(usuarioId, recursoId, diasPlazo, DateTime.UtcNow);

            recurso.DisminuirStock(); // bajamos el stock
            await _prestamoRepository.AddAsync(prestamo);
            _recursoRepository.Update(recurso);
            
            await _unitOfWork.SaveChangesAsync();

            // Notificacion proactiva (como dice la documentacion)
            try 
            {
                await _emailAdapter.EnviarAsync(usuario.Correo, "Confirmación de Préstamo - SIGEBI", 
                    $"Hola {usuario.Nombre}, se ha registrado tu préstamo del recurso: {recurso.Titulo}. " +
                    $"Fecha de devolución: {prestamo.FechaDevolucionEstimada:dd/MM/yyyy}.");
            }
            catch 
            {
                // Si falla el mail no rompemos la transaccion que ya se guardo, 
                // pero lo ideal seria un log aca.
            }

            return PrestamoMapper.ToDTO(prestamo);
        }
    }
}
