using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using SIGEBI.Business.DTOs;

namespace SIGEBI.Web.Services
{
    public interface ICategoriasApi
    {
        [Get("/api/Categorias")]
        Task<List<CategoriaDTO>> GetCategoriasAsync();
    }
}
