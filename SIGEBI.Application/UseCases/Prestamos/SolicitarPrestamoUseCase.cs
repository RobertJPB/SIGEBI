using SIGEBI.Business.DTOs;
using SIGEBI.Application.Interfaces;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Business.Mappers;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.UseCases.Prestamos
    // Principio SOLID (DIP - Inversión de Dependencias):
    // Esta clase NO conoce a SQL Server ni a Entity Framework. 
    // Solo conoce las interfaces (IPrestamoRepository, etc) que le inyectaron por constructor.
    public class SolicitarPrestamoUseCase
    {
        private readonly IPrestamoRepository _prestamoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IRecursoRepository _recursoRepository;
        private readonly IPenalizacionRepository _penalizacionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SolicitarPrestamoUseCase(
            IPrestamoRepository prestamoRepository,
            IUsuarioRepository usuarioRepository,
            IRecursoRepository recursoRepository,
            IPenalizacionRepository penalizacionRepository,
            IUnitOfWork unitOfWork)
        {
            _prestamoRepository = prestamoRepository;
            _usuarioRepository = usuarioRepository;
            _recursoRepository = recursoRepository;
            _penalizacionRepository = penalizacionRepository;
            _unitOfWork = unitOfWork;
        }

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

            return PrestamoMapper.ToDTO(prestamo);
        }
    }
}