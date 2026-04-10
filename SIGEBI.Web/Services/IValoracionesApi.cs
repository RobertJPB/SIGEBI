using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using SIGEBI.Business.DTOs;

namespace SIGEBI.Web.Services
{
    public interface IValoracionesApi
    {
        [Get("/api/Valoraciones/recurso/{recursoId}")]
        Task<List<ValoracionDTO>> GetValoracionesAsync(Guid recursoId, [Header("Authorization")] string token);

        [Post("/api/Valoraciones")]
        Task ValorarAsync([Body] object request, [Header("Authorization")] string token);

        [Delete("/api/Valoraciones/{id}")]
        Task EliminarValoracionAsync(Guid id, [Header("Authorization")] string token);
    }
}
