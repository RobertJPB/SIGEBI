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
        Task<RecursoDetalleDTO> AgregarLibroAsync(LibroRequestDTO request);
        Task<RecursoDetalleDTO> AgregarRevistaAsync(RevistaRequestDTO request);
        Task<RecursoDetalleDTO> AgregarDocumentoAsync(DocumentoRequestDTO request);

        // ── EDITAR ──
        Task<RecursoDetalleDTO> EditarLibroAsync(Guid id, LibroRequestDTO request);
        Task<RecursoDetalleDTO> EditarRevistaAsync(Guid id, RevistaRequestDTO request);
        Task<RecursoDetalleDTO> EditarDocumentoAsync(Guid id, DocumentoRequestDTO request);

        Task ActualizarImagenAsync(Guid recursoId, string imagenUrl);

        // ── ELIMINAR ──
        Task EliminarRecursoAsync(Guid recursoId);
    }
}
