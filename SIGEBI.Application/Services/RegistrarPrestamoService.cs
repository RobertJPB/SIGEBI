using SIGEBI.Business.DTOs;
using SIGEBI.Business.UseCases.Prestamos;

namespace SIGEBI.Business.Services
{
    public class RegistrarPrestamoService
    {
        private readonly SolicitarPrestamoUseCase _solicitarUseCase;
        private readonly DevolverPrestamoUseCase _devolverUseCase;
        private readonly ConsultarPrestamoUseCase _consultarUseCase;

        public RegistrarPrestamoService(
            SolicitarPrestamoUseCase solicitarUseCase,
            DevolverPrestamoUseCase devolverUseCase,
            ConsultarPrestamoUseCase consultarUseCase)
        {
            _solicitarUseCase = solicitarUseCase;
            _devolverUseCase = devolverUseCase;
            _consultarUseCase = consultarUseCase;
        }

        // Usando un Facade para no inyectar tantos UseCases en el Controller (visto en clase)
        public async Task<PrestamoResponseDTO> SolicitarPrestamoAsync(Guid usuarioId, Guid recursoId)
            => await _solicitarUseCase.EjecutarAsync(usuarioId, recursoId);

        public async Task DevolverPrestamoAsync(Guid prestamoId)
            => await _devolverUseCase.EjecutarAsync(prestamoId);

        public async Task<IEnumerable<PrestamoResponseDTO>> ObtenerTodosAsync()
            => await _consultarUseCase.ObtenerTodosAsync();

        public async Task<IEnumerable<PrestamoResponseDTO>> ObtenerPorUsuarioAsync(Guid usuarioId)
            => await _consultarUseCase.ObtenerPorUsuarioAsync(usuarioId);

        public async Task<IEnumerable<PrestamoResponseDTO>> ObtenerActivosPorUsuarioAsync(Guid usuarioId)
            => await _consultarUseCase.ObtenerActivosPorUsuarioAsync(usuarioId);

        public async Task<IEnumerable<PrestamoResponseDTO>> ObtenerAtrasadosAsync()
            => await _consultarUseCase.ObtenerAtrasadosAsync();
    }
}
