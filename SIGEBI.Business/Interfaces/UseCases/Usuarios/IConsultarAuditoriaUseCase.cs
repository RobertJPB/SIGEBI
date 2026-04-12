using SIGEBI.Business.DTOs;

namespace SIGEBI.Business.Interfaces.UseCases.Usuarios
{
    public interface IConsultarAuditoriaUseCase
    {
        Task<IEnumerable<AuditoriaDTO>> ObtenerTodasAsync();
        Task<IEnumerable<AuditoriaDTO>> ObtenerPorUsuarioAsync(Guid usuarioId);
        Task<IEnumerable<AuditoriaDTO>> ObtenerPorEntidadAsync(string entidad, string entidadId);
    }
}

