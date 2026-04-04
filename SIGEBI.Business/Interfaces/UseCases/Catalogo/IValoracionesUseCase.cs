using SIGEBI.Business.DTOs;

namespace SIGEBI.Business.Interfaces.UseCases.Catalogo
{
    /// <summary>
    /// Contrato para la gestión de calificaciones y comentarios de usuarios
    /// sobre recursos bibliográficos.
    /// </summary>
    public interface IValoracionesUseCase
    {
        Task<ValoracionDTO> AgregarValoracionAsync(Guid usuarioId, Guid recursoId, int calificacion, string? comentario);
        Task<IEnumerable<ValoracionDTO>> ObtenerValoracionesPorRecursoAsync(Guid recursoId);
        Task<ValoracionDTO> ObtenerPorIdAsync(Guid id);
        Task<double> ObtenerPromedioAsync(Guid recursoId);
        Task EliminarValoracionAsync(Guid id);
    }
}
