using SIGEBI.Business.DTOs;

namespace SIGEBI.Business.Interfaces.UseCases.Catalogo
{
    public interface IConsultarLibrosUseCase
    {
        Task<IEnumerable<RecursoDetalleDTO>> EjecutarAsync();
        Task<IEnumerable<RecursoDetalleDTO>> BuscarPorTituloAsync(string titulo);
        Task<IEnumerable<RecursoDetalleDTO>> BuscarPorCategoriaAsync(int categoriaId);
        Task<RecursoDetalleDTO?> GetByIdAsync(Guid id);
    }
}

