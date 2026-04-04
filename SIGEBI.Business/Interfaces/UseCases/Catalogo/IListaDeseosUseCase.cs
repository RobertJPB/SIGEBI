using SIGEBI.Business.DTOs;

namespace SIGEBI.Business.Interfaces.UseCases.Catalogo
{
    /// <summary>
    /// Contrato para la gestión de la lista de deseos de un usuario.
    /// </summary>
    public interface IListaDeseosUseCase
    {
        Task<ListaDeseosDTO> ObtenerPorUsuarioAsync(Guid usuarioId);
        Task AgregarRecursoAsync(Guid usuarioId, Guid recursoId);
        Task RemoverRecursoAsync(Guid usuarioId, Guid recursoId);
    }
}
