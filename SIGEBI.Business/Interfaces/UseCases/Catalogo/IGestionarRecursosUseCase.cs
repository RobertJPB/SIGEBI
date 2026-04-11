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
        Task<RecursoDetalleDTO> AgregarLibroAsync(LibroRequestDTO request, Guid usuarioCreadorId);
        Task<RecursoDetalleDTO> AgregarRevistaAsync(RevistaRequestDTO request, Guid usuarioCreadorId);
        Task<RecursoDetalleDTO> AgregarDocumentoAsync(DocumentoRequestDTO request, Guid usuarioCreadorId);

        // ── EDITAR ──
        Task<RecursoDetalleDTO> EditarLibroAsync(Guid id, LibroRequestDTO request);
        Task<RecursoDetalleDTO> EditarRevistaAsync(Guid id, RevistaRequestDTO request);
        Task<RecursoDetalleDTO> EditarDocumentoAsync(Guid id, DocumentoRequestDTO request);

        Task ActualizarImagenAsync(Guid recursoId, string imagenUrl);
        Task EliminarRecursoAsync(Guid id);

        // ── UTILIDADES ──
        Task<IEnumerable<string>> ObtenerAutoresAsync();
        Task<IEnumerable<string>> ObtenerEditorialesAsync();
    }
}

