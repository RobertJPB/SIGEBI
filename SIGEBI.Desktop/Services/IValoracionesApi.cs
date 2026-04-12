using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using SIGEBI.Business.DTOs;

namespace SIGEBI.Services
{
    public interface IValoracionesApi
    {
        [Get("/api/Valoraciones/recurso/{recursoId}")]
        Task<List<ValoracionDTO>> GetValoracionesAsync(Guid recursoId);

        [Post("/api/Valoraciones")]
        Task ValorarAsync([Body] ValoracionDTO request);

        [Delete("/api/Valoraciones/{id}")]
        Task EliminarValoracionAsync(Guid id);
    }
}
