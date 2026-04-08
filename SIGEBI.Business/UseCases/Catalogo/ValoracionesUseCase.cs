using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.Mappers;
using SIGEBI.Business.Interfaces.Common;
using SIGEBI.Business.Interfaces.UseCases.Catalogo;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.UseCases.Catalogo
{
    // Maneja las calificaciones y comentarios sociales de los usuarios sobre el material bibliográfico.
    public class ValoracionesUseCase : IValoracionesUseCase
    {
        private readonly IValoracionRepository _valoracionRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IRecursoRepository _recursoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGuidGenerator _guidGenerator;

        public ValoracionesUseCase(
            IValoracionRepository valoracionRepository,
            IUsuarioRepository usuarioRepository,
            IRecursoRepository recursoRepository,
            IUnitOfWork unitOfWork,
            IGuidGenerator guidGenerator)
        {
            _valoracionRepository = valoracionRepository;
            _usuarioRepository = usuarioRepository;
            _recursoRepository = recursoRepository;
            _unitOfWork = unitOfWork;
            _guidGenerator = guidGenerator;
        }

        // Crea una nueva valoración validando la existencia del usuario y el recurso.
        public async Task<ValoracionDTO> AgregarValoracionAsync(Guid usuarioId, Guid recursoId,
            int calificacion, string? comentario)
        {
            // Validaciones iniciales
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId)
                ?? throw new KeyNotFoundException("Usuario no encontrado.");

            var recurso = await _recursoRepository.GetByIdAsync(recursoId)
                ?? throw new KeyNotFoundException("Recurso no encontrado.");

            // Guardamos la calificacion (estrellas) que puso el usuario
            var valoracion = new Valoracion(
                _guidGenerator.Create(),
                usuarioId,
                recursoId,
                calificacion,
                comentario
            );
            
            await _valoracionRepository.AddAsync(valoracion);
            await _unitOfWork.SaveChangesAsync();
            
            return ValoracionMapper.ToDTO(valoracion);
        }

        public async Task<IEnumerable<ValoracionDTO>> ObtenerValoracionesPorRecursoAsync(Guid recursoId)
        {
            var valoraciones = await _valoracionRepository.GetByRecursoIdAsync(recursoId);
            return valoraciones.Select(ValoracionMapper.ToDTO);
        }

        public async Task<ValoracionDTO> ObtenerPorIdAsync(Guid id)
        {
            var valoracion = await _valoracionRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Valoración no encontrada.");
            return ValoracionMapper.ToDTO(valoracion);
        }

        // Calcula la puntuación media basada en todas las estrellas recibidas por el recurso.
        public async Task<double> ObtenerPromedioAsync(Guid recursoId)
            => await _valoracionRepository.GetPromedioCalificacionAsync(recursoId);

        public async Task EliminarValoracionAsync(Guid id)
        {
            var valoracion = await _valoracionRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Valoración no encontrada.");

            _valoracionRepository.Delete(valoracion);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
