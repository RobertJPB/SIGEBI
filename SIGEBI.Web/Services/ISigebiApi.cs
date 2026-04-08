using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Refit;
using SIGEBI.Business.DTOs;

namespace SIGEBI.Web.Services
{
    public interface ISigebiApi
    {
        // ── CATEGORÍAS ──
        [Get("/api/Categorias")]
        Task<List<CategoriaDTO>> GetCategoriasAsync();

        // ── RECURSOS ──
        [Get("/api/Recursos")]
        Task<List<RecursoDetalleDTO>> GetRecursosAsync([Header("Authorization")] string token);

        [Get("/api/Recursos/{id}")]
        Task<RecursoDetalleDTO> GetRecursoAsync(Guid id, [Header("Authorization")] string token);

        [Get("/api/Recursos/buscar")]
        Task<List<RecursoDetalleDTO>> BuscarRecursosAsync([Query] string titulo, [Header("Authorization")] string token);

        // ── VALORACIONES ──
        [Get("/api/Valoraciones/recurso/{recursoId}")]
        Task<List<ValoracionDTO>> GetValoracionesAsync(Guid recursoId, [Header("Authorization")] string token);

        [Post("/api/Valoraciones")]
        Task ValorarAsync([Body] object request, [Header("Authorization")] string token);

        [Delete("/api/Valoraciones/{id}")]
        Task EliminarValoracionAsync(Guid id, [Header("Authorization")] string token);

        // ── PRÉSTAMOS ──
        [Get("/api/Prestamos/usuario/{usuarioId}")]
        Task<List<PrestamoResponseDTO>> GetPrestamosByUsuarioAsync(Guid usuarioId, [Header("Authorization")] string token);

        [Post("/api/Prestamos")]
        Task SolicitarPrestamoAsync([Body] object request, [Header("Authorization")] string token);

        // ── LISTA DESEOS ──
        [Post("/api/ListaDeseos/usuario/{usuarioId}/recurso/{recursoId}")]
        Task AgregarAListaDeseosAsync(Guid usuarioId, Guid recursoId, [Header("Authorization")] string token);
        
        // ── AUTH ──
        [Post("/api/Auth/login")]
        Task<JsonElement> LoginAsync([Body] object request);
    }
}
