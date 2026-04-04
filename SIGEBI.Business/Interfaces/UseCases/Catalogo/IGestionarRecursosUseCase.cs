using SIGEBI.Business.DTOs;

namespace SIGEBI.Business.Interfaces.UseCases.Catalogo
{
    /// <summary>
    /// Contrato para la creación, edición y eliminación de recursos bibliográficos
    /// (libros, revistas y documentos).
    /// </summary>
    public interface IGestionarRecursosUseCase
    {
        // ── AGREGAR ──
        Task<RecursoDetalleDTO> AgregarLibroAsync(string titulo, string autor,
            int categoriaId, int stock, string? descripcion, string isbn,
            string editorial, int anio, string? imagenUrl = null, string? genero = null);

        Task<RecursoDetalleDTO> AgregarRevistaAsync(string titulo, string autor,
            int categoriaId, int stock, string? descripcion, int numeroEdicion,
            string issn, int anio, string? editorial, string? imagenUrl = null);

        Task<RecursoDetalleDTO> AgregarDocumentoAsync(string titulo, string autor,
            int categoriaId, int stock, string? descripcion, string formato,
            string institucion, int anio, string? imagenUrl = null);

        // ── EDITAR ──
        Task<RecursoDetalleDTO> EditarLibroAsync(Guid id, string titulo, string autor,
            int categoriaId, int stock, string? descripcion, string isbn,
            string editorial, int anio, string? imagenUrl = null, string? genero = null);

        Task<RecursoDetalleDTO> EditarRevistaAsync(Guid id, string titulo, string autor,
            int categoriaId, int stock, string? descripcion, int numeroEdicion,
            string issn, int anio, string? editorial, string? imagenUrl = null);

        Task<RecursoDetalleDTO> EditarDocumentoAsync(Guid id, string titulo, string autor,
            int categoriaId, int stock, string? descripcion, string formato,
            string institucion, int anio, string? imagenUrl = null);

        Task ActualizarImagenAsync(Guid recursoId, string imagenUrl);

        // ── ELIMINAR ──
        Task EliminarRecursoAsync(Guid recursoId);
    }
}
