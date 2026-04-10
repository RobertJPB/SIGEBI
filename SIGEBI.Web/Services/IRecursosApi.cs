using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using SIGEBI.Business.DTOs;

namespace SIGEBI.Web.Services
{
    public interface IRecursosApi
    {
        [Get("/api/Recursos")]
        Task<List<RecursoDetalleDTO>> GetRecursosAsync([Header("Authorization")] string token);

        [Get("/api/Recursos/{id}")]
        Task<RecursoDetalleDTO> GetRecursoAsync(Guid id, [Header("Authorization")] string token);

        [Get("/api/Recursos/buscar")]
        Task<List<RecursoDetalleDTO>> BuscarRecursosAsync([Query] string titulo, [Header("Authorization")] string token);
    }
}
