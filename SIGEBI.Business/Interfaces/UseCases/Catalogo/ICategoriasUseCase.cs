using SIGEBI.Business.DTOs;

namespace SIGEBI.Business.Interfaces.UseCases.Catalogo
{
    public interface ICategoriasUseCase
    {
        Task<IEnumerable<CategoriaDTO>> ObtenerTodasAsync();
        Task<IEnumerable<CategoriaDTO>> ObtenerActivasAsync();
        Task<CategoriaDTO> ObtenerPorIdAsync(int id);
        Task<CategoriaDTO> CrearAsync(string nombre);
        Task<CategoriaDTO> ActualizarAsync(int id, string nuevoNombre);
        Task EliminarAsync(int id);
    }
}

