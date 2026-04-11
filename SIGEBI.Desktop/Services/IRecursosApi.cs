using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using SIGEBI.Business.DTOs;

namespace SIGEBI.Services
{
    public interface IRecursosApi
    {
        [Get("/api/Recursos")]
        Task<List<RecursoDetalleDTO>> GetRecursosAsync();

        [Get("/api/Recursos/buscar")]
        Task<List<RecursoDetalleDTO>> BuscarRecursosPorTituloAsync([Query] string titulo);

        [Delete("/api/Recursos/{id}")]
        Task EliminarRecursoAsync(Guid id);
    }

    public interface ICategoriasApi
    {
        [Get("/api/Categorias")]
        Task<List<CategoriaDTO>> GetCategoriasAsync();
    }
}
