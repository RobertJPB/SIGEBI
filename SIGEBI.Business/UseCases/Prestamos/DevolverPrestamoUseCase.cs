using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.UseCases.Prestamos
{
    // Orquesta el proceso de retorno de un recurso y el cálculo de penalizaciones por atraso.
    public class DevolverPrestamoUseCase
    {
        private readonly IPrestamoRepository _prestamoRepository;
        private readonly IRecursoRepository _recursoRepository;
        private readonly IPenalizacionRepository _penalizacionRepository;
        private readonly INotificacionRepository _notificacionRepository;
        private readonly IListaDeseosRepository _listaDeseosRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DevolverPrestamoUseCase(
            IPrestamoRepository prestamoRepository,
            IRecursoRepository recursoRepository,
            IPenalizacionRepository penalizacionRepository,
            INotificacionRepository notificacionRepository,
            IListaDeseosRepository listaDeseosRepository,
            IUnitOfWork unitOfWork)
        {
            _prestamoRepository = prestamoRepository;
            _recursoRepository = recursoRepository;
            _penalizacionRepository = penalizacionRepository;
            _notificacionRepository = notificacionRepository;
            _listaDeseosRepository = listaDeseosRepository;
            _unitOfWork = unitOfWork;
        }

        // Registra la devolución real, repone el stock y genera multas si el plazo venció.
        public async Task EjecutarAsync(Guid prestamoId)
        {
            var prestamo = await _prestamoRepository.GetByIdAsync(prestamoId)
                ?? throw new InvalidOperationException("Préstamo no encontrado.");

            var recurso = await _recursoRepository.GetByIdAsync(prestamo.RecursoId)
                ?? throw new InvalidOperationException("Recurso no encontrado.");

            bool estabaAgotado = recurso.Stock == 0;
            
            prestamo.Devolver(DateTime.UtcNow);
            recurso.AumentarStock(); // el libro vuelve a estar disponible

            // Si estaba agotado y ahora hay stock, avisamos a los interesados de la lista de deseos
            if (estabaAgotado && recurso.Stock > 0)
            {
                var usuariosInteresados = await _listaDeseosRepository.GetUsuariosInteresadosAsync(recurso.Id);
                foreach (var usuarioId in usuariosInteresados)
                {
                    // No avisamos al que lo acaba de devolver (innecesario)
                    if (usuarioId == prestamo.UsuarioId) continue;

                    var notiDisponibilidad = NotificacionFactory.CrearNotificacionDisponibilidad(usuarioId, recurso.Titulo);
                    await _notificacionRepository.AddAsync(notiDisponibilidad);
                }
            }

            // Notificación de devolución exitosa
            var notiDevolucion = NotificacionFactory.CrearNotificacionDevolucion(prestamo.UsuarioId);
            await _notificacionRepository.AddAsync(notiDevolucion);

            // Verificamos si lo entrego tarde para clavarle una multa
            if (PenalizacionCalculator.TienePenalizacion(prestamo.FechaDevolucionEstimada, DateTime.UtcNow))
            {
                int diasAtraso = (int)(DateTime.UtcNow - prestamo.FechaDevolucionEstimada).TotalDays;
                int diasPenalizacion = PenalizacionCalculator.CalcularDiasPenalizacion(
                    prestamo.FechaDevolucionEstimada, DateTime.UtcNow);
                
                string motivo = PenalizacionCalculator.ObtenerMotivo(diasAtraso);

                var penalizacion = new Penalizacion(prestamo.UsuarioId, motivo, diasPenalizacion, DateTime.UtcNow);
                await _penalizacionRepository.AddAsync(penalizacion);

                // Notificación de penalización
                var notiPenalizacion = NotificacionFactory.CrearNotificacionPenalizacion(
                    prestamo.UsuarioId, motivo, penalizacion.FechaFin.Value);
                await _notificacionRepository.AddAsync(notiPenalizacion);
            }

            _prestamoRepository.Update(prestamo);
            _recursoRepository.Update(recurso);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
