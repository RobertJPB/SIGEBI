using SIGEBI.Business.DTOs;

namespace SIGEBI.Business.Interfaces.UseCases.Catalogo
{
    /// <summary>
    /// Contrato para la consulta y búsqueda de recursos bibliográficos disponibles.
    /// </summary>
    public interface IConsultarLibrosUseCase
    {
        Task<IEnumerable<RecursoDetalleDTO>> EjecutarAsync();
        Task<IEnumerable<RecursoDetalleDTO>> BuscarPorTituloAsync(string titulo);
        Task<IEnumerable<RecursoDetalleDTO>> BuscarPorCategoriaAsync(int categoriaId);
        Task<RecursoDetalleDTO?> GetByIdAsync(Guid id);
    }
}
