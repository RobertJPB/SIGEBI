using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.Interfaces.UseCases.Prestamos;
using SIGEBI.Business.Mappers;

namespace SIGEBI.Business.UseCases.Prestamos
{
    // Provee métodos de lectura para consultar el historial y estado de los préstamos.
    public class ConsultarPrestamoUseCase : IConsultarPrestamoUseCase
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

        // Recupera la lista global de préstamos registrados en el sistema.
        public async Task<IEnumerable<PrestamoResponseDTO>> ObtenerTodosAsync()
        {
            var prestamos = await _prestamoRepository.GetAllAsync();
            return prestamos.Select(PrestamoMapper.ToDTO);
        }

        // Filtra los préstamos pertenecientes a un usuario en particular.
        public async Task<IEnumerable<PrestamoResponseDTO>> ObtenerPorUsuarioAsync(Guid usuarioId)
        {
            // Verificamos que el id sea de alguien real
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

        // Lista todos los préstamos que han superado su fecha límite de devolución.
        public async Task<IEnumerable<PrestamoResponseDTO>> ObtenerAtrasadosAsync()
        {
            var prestamos = await _prestamoRepository.GetAtrasadosAsync();
            return prestamos.Select(PrestamoMapper.ToDTO);
        }
    }
}