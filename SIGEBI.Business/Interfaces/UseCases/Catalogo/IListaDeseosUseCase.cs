using SIGEBI.Business.DTOs;

namespace SIGEBI.Business.Interfaces.UseCases.Catalogo
{
    public interface IListaDeseosUseCase
    {
        Task<ListaDeseosDTO> ObtenerPorUsuarioAsync(Guid usuarioId);
        Task AgregarRecursoAsync(Guid usuarioId, Guid recursoId);
        Task RemoverRecursoAsync(Guid usuarioId, Guid recursoId);
    }
}

