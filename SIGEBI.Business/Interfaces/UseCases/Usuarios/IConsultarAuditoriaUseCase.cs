using SIGEBI.Business.DTOs;

namespace SIGEBI.Business.Interfaces.UseCases.Usuarios
{
    /// <summary>
    /// Contrato para la consulta de registros de auditoría del sistema.
    /// </summary>
    public interface IConsultarAuditoriaUseCase
    {
        Task<IEnumerable<AuditoriaDTO>> ObtenerTodasAsync();
        Task<IEnumerable<AuditoriaDTO>> ObtenerPorUsuarioAsync(Guid usuarioId);
    }
}
