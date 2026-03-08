using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Business.Mappers;

namespace SIGEBI.Business.UseCases.Prestamos
{
    public class ConsultarPrestamoUseCase
    {
        private readonly IPrestamoRepository _prestamoRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public ConsultarPrestamoUseCase(
            IPrestamoRepository prestamoRepository,
            IUsuarioRepository usuarioRepository)
        {
            _prestamoRepository = prestamoRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<IEnumerable<PrestamoResponseDTO>> ObtenerTodosAsync()
        {
            var prestamos = await _prestamoRepository.GetAllAsync();
            return prestamos.Select(PrestamoMapper.ToDTO);
        }

        public async Task<IEnumerable<PrestamoResponseDTO>> ObtenerPorUsuarioAsync(Guid usuarioId)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId)
                ?? throw new InvalidOperationException("Usuario no encontrado.");

            var prestamos = await _prestamoRepository.GetByUsuarioIdAsync(usuarioId);
            return prestamos.Select(PrestamoMapper.ToDTO);
        }

        public async Task<IEnumerable<PrestamoResponseDTO>> ObtenerActivosPorUsuarioAsync(Guid usuarioId)
        {
            var prestamos = await _prestamoRepository.GetActivosByUsuarioIdAsync(usuarioId);
            return prestamos.Select(PrestamoMapper.ToDTO);
        }

        public async Task<IEnumerable<PrestamoResponseDTO>> ObtenerAtrasadosAsync()
        {
            var prestamos = await _prestamoRepository.GetAtrasadosAsync();
            return prestamos.Select(PrestamoMapper.ToDTO);
        }
    }
}