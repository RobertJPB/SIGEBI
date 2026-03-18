using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Business.Mappers;

namespace SIGEBI.Business.UseCases.Usuarios
{
    public class ConsultarAuditoriaUseCase
    {
        private readonly IAuditoriaRepository _auditoriaRepository;

        public ConsultarAuditoriaUseCase(IAuditoriaRepository auditoriaRepository)
        {
            _auditoriaRepository = auditoriaRepository;
        }

        public async Task<IEnumerable<AuditoriaDTO>> ObtenerTodasAsync()
        {
            var auditorias = await _auditoriaRepository.GetAllAsync();
            return auditorias.Select(AuditoriaMapper.ToDTO);
        }

        public async Task<IEnumerable<AuditoriaDTO>> ObtenerPorUsuarioAsync(Guid usuarioId)
        {
            // Trae todo el historial de lo que hizo un usuario
            var auditorias = await _auditoriaRepository.GetByUsuarioIdAsync(usuarioId);
            return auditorias.Select(AuditoriaMapper.ToDTO);
        }
    }
}