using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Business.Mappers;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.UseCases.Catalogo
{
    public class ValoracionesUseCase
    {
        private readonly IValoracionRepository _valoracionRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IRecursoRepository _recursoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ValoracionesUseCase(
            IValoracionRepository valoracionRepository,
            IUsuarioRepository usuarioRepository,
            IRecursoRepository recursoRepository,
            IUnitOfWork unitOfWork)
        {
            _valoracionRepository = valoracionRepository;
            _usuarioRepository = usuarioRepository;
            _recursoRepository = recursoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ValoracionDTO> AgregarValoracionAsync(Guid usuarioId, Guid recursoId,
            int calificacion, string? comentario)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId)
                ?? throw new InvalidOperationException("Usuario no encontrado.");

            var recurso = await _recursoRepository.GetByIdAsync(recursoId)
                ?? throw new InvalidOperationException("Recurso no encontrado.");

            var valoracion = new Valoracion(usuarioId, recursoId, calificacion, comentario);
            await _valoracionRepository.AddAsync(valoracion);
            await _unitOfWork.SaveChangesAsync();
            return ValoracionMapper.ToDTO(valoracion);
        }

        public async Task<IEnumerable<ValoracionDTO>> ObtenerValoracionesPorRecursoAsync(Guid recursoId)
        {
            var valoraciones = await _valoracionRepository.GetByRecursoIdAsync(recursoId);
            return valoraciones.Select(ValoracionMapper.ToDTO);
        }

        public async Task<double> ObtenerPromedioAsync(Guid recursoId)
            => await _valoracionRepository.GetPromedioCalificacionAsync(recursoId);
    }
}