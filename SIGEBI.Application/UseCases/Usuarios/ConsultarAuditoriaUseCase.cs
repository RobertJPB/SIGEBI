using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Business.Mappers;

namespace SIGEBI.Business.UseCases.Usuarios
{
    // Permite la consulta y filtrado de los registros de auditoría del sistema.
    public class ConsultarAuditoriaUseCase
    {
        private readonly IAuditoriaRepository _auditoriaRepository;
        private readonly SIGEBI.Application.Interfaces.IUnitOfWork _unitOfWork;

        public ConsultarAuditoriaUseCase(
            IAuditoriaRepository auditoriaRepository,
            SIGEBI.Application.Interfaces.IUnitOfWork unitOfWork)
        {
            _auditoriaRepository = auditoriaRepository;
            _unitOfWork = unitOfWork;
        }

        // Obtiene el historial completo de acciones registradas en la base de datos.
        public async Task<IEnumerable<AuditoriaDTO>> ObtenerTodasAsync()
        {
            var auditorias = await _auditoriaRepository.GetAllAsync();
            return auditorias.Select(AuditoriaMapper.ToDTO);
        }

        // Filtra los eventos de auditoría realizados por un usuario específico.
        public async Task<IEnumerable<AuditoriaDTO>> ObtenerPorUsuarioAsync(Guid usuarioId)
        {
            // Trae todo el historial de lo que hizo un usuario
            var auditorias = await _auditoriaRepository.GetByUsuarioIdAsync(usuarioId);
            return auditorias.Select(AuditoriaMapper.ToDTO);
        }

        // Elimina permanentemente un registro de auditoría.
        public async Task EliminarAuditoriaAsync(int id)
        {
            var auditoria = await _auditoriaRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Auditoría no encontrada.");

            _auditoriaRepository.Delete(auditoria);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}