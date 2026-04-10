using SIGEBI.Business.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIGEBI.Web.Services
{
    public interface ICatalogoService
    {
        Task<List<RecursoDetalleDTO>> GetRecursosAsync(string token, string? busqueda = null);
        Task<RecursoDetalleDTO> GetRecursoAsync(Guid id, string token);
        Task<List<ValoracionDTO>> GetValoracionesAsync(Guid recursoId, string token);
        Task ValorarAsync(Guid usuarioId, Guid recursoId, int calificacion, string comentario, string token);
        Task EliminarValoracionAsync(Guid id, string token);
        Task AgregarAListaDeseosAsync(Guid usuarioId, Guid recursoId, string token);
        Task SolicitarPrestamoAsync(Guid usuarioId, Guid recursoId, string token);
    }
}
