using Microsoft.Extensions.Caching.Memory;
using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.Mappers;
using SIGEBI.Business.Interfaces.Common;
using SIGEBI.Business.Interfaces.UseCases.Catalogo;
using SIGEBI.Domain.Entities.Recursos;

namespace SIGEBI.Business.UseCases.Catalogo
{
    // Orquesta la creación, edición y eliminación lógica de libros, revistas y documentos.
    public class GestionarRecursosUseCase : IGestionarRecursosUseCase
    {
        private readonly IRecursoRepository _recursoRepository;
        private readonly IPrestamoRepository _prestamoRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;
        private readonly IGuidGenerator _guidGenerator;

        public GestionarRecursosUseCase(
            IRecursoRepository recursoRepository,
            IPrestamoRepository prestamoRepository,
            ICategoriaRepository categoriaRepository,
            IUnitOfWork unitOfWork,
            IMemoryCache cache,
            IGuidGenerator guidGenerator)
        {
            _recursoRepository = recursoRepository;
            _prestamoRepository = prestamoRepository;
            _categoriaRepository = categoriaRepository;
            _unitOfWork = unitOfWork;
            _cache = cache;
            _guidGenerator = guidGenerator;
        }

        private void InvalidateCache()
        {
            _cache.Remove("RecursosDisponibles");
        }

        // ── AGREGAR ──

        // Registra un nuevo libro validando su categoría y persistiendo sus datos específicos (ISBN, etc).
        public async Task<RecursoDetalleDTO> AgregarLibroAsync(string titulo, string autor,
            int categoriaId, int stock, string? descripcion, string isbn, string editorial, int anio,
            string? imagenUrl = null, string? genero = null)
        {
            // Ojo: validar que la categoria exista primero
            var categoria = await _categoriaRepository.GetByIdAsync(categoriaId)
                ?? throw new InvalidOperationException("Categoría no encontrada.");
            var libro = new Libro(_guidGenerator.Create(), titulo, autor, categoriaId, stock, descripcion, isbn, editorial, anio, genero);
            if (imagenUrl != null) libro.ActualizarImagen(imagenUrl);
            await _recursoRepository.AddAsync(libro);
            await _unitOfWork.SaveChangesAsync();
            
            InvalidateCache();
            return RecursoMapper.ToDTO(libro);
        }

        public async Task<RecursoDetalleDTO> AgregarRevistaAsync(string titulo, string autor,
            int categoriaId, int stock, string? descripcion, int numeroEdicion, string issn, int anio, string? editorial,
            string? imagenUrl = null)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(categoriaId)
                ?? throw new InvalidOperationException("Categoría no encontrada.");
            var revista = new Revista(_guidGenerator.Create(), titulo, autor, categoriaId, stock, descripcion, numeroEdicion, issn, anio, editorial);
            if (imagenUrl != null) revista.ActualizarImagen(imagenUrl);
            await _recursoRepository.AddAsync(revista);
            await _unitOfWork.SaveChangesAsync();
            
            InvalidateCache();
            return RecursoMapper.ToDTO(revista);
        }

        public async Task<RecursoDetalleDTO> AgregarDocumentoAsync(string titulo, string autor,
            int categoriaId, int stock, string? descripcion, string formato, string institucion, int anio,
            string? imagenUrl = null)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(categoriaId)
                ?? throw new InvalidOperationException("Categoría no encontrada.");
            var documento = new Documento(_guidGenerator.Create(), titulo, autor, categoriaId, stock, descripcion, formato, institucion, anio);
            if (imagenUrl != null) documento.ActualizarImagen(imagenUrl);
            await _recursoRepository.AddAsync(documento);
            await _unitOfWork.SaveChangesAsync();
            
            InvalidateCache();
            return RecursoMapper.ToDTO(documento);
        }

        // ── EDITAR ──

        // Actualiza la información de un libro existente en el sistema.
        public async Task<RecursoDetalleDTO> EditarLibroAsync(Guid id, string titulo, string autor,
            int categoriaId, int stock, string? descripcion, string isbn, string editorial, int anio,
            string? imagenUrl = null, string? genero = null)
        {
            var libro = await _recursoRepository.GetByIdAsync(id) as Libro
                ?? throw new KeyNotFoundException("Libro no encontrado.");
            var categoria = await _categoriaRepository.GetByIdAsync(categoriaId)
                ?? throw new InvalidOperationException("Categoría no encontrada.");
            libro.Actualizar(titulo, autor, categoriaId, stock, descripcion, isbn, editorial, anio, genero);
            if (imagenUrl != null) libro.ActualizarImagen(imagenUrl);
            _recursoRepository.Update(libro);
            await _unitOfWork.SaveChangesAsync();
            
            InvalidateCache();
            return RecursoMapper.ToDTO(libro);
        }

        public async Task<RecursoDetalleDTO> EditarRevistaAsync(Guid id, string titulo, string autor,
            int categoriaId, int stock, string? descripcion, int numeroEdicion, string issn, int anio, string? editorial,
            string? imagenUrl = null)
        {
            var revista = await _recursoRepository.GetByIdAsync(id) as Revista
                ?? throw new InvalidOperationException("Revista no encontrada.");
            var categoria = await _categoriaRepository.GetByIdAsync(categoriaId)
                ?? throw new InvalidOperationException("Categoría no encontrada.");
            revista.Actualizar(titulo, autor, categoriaId, stock, descripcion, numeroEdicion, issn, anio, editorial);
            if (imagenUrl != null) revista.ActualizarImagen(imagenUrl);
            _recursoRepository.Update(revista);
            await _unitOfWork.SaveChangesAsync();
            
            InvalidateCache();
            return RecursoMapper.ToDTO(revista);
        }

        public async Task<RecursoDetalleDTO> EditarDocumentoAsync(Guid id, string titulo, string autor,
            int categoriaId, int stock, string? descripcion, string formato, string institucion, int anio,
            string? imagenUrl = null)
        {
            var documento = await _recursoRepository.GetByIdAsync(id) as Documento
                ?? throw new KeyNotFoundException("Documento no encontrado.");
            var categoria = await _categoriaRepository.GetByIdAsync(categoriaId)
                ?? throw new InvalidOperationException("Categoría no encontrada.");
            documento.Actualizar(titulo, autor, categoriaId, stock, descripcion, formato, institucion, anio);
            if (imagenUrl != null) documento.ActualizarImagen(imagenUrl);
            _recursoRepository.Update(documento);
            await _unitOfWork.SaveChangesAsync();
            
            InvalidateCache();
            return RecursoMapper.ToDTO(documento);
        }

  

        public async Task ActualizarImagenAsync(Guid recursoId, string imagenUrl)
        {
            var recurso = await _recursoRepository.GetByIdAsync(recursoId)
                ?? throw new KeyNotFoundException("Recurso no encontrado.");
            recurso.ActualizarImagen(imagenUrl);
            _recursoRepository.Update(recurso);
            await _unitOfWork.SaveChangesAsync();
            
            InvalidateCache();
        }

        // Elimina permanentemente el recurso del sistema (hard delete).
        public async Task EliminarRecursoAsync(Guid recursoId)
        {
            var recurso = await _recursoRepository.GetByIdAsync(recursoId)
                ?? throw new KeyNotFoundException("Recurso no encontrado.");

            // REGLA: No se permite la eliminación de recursos con préstamos activos.
            var prestamosActivos = await _prestamoRepository.GetActivosByRecursoIdAsync(recursoId);
            if (prestamosActivos.Any())
            {
                throw new InvalidOperationException("No se puede eliminar el recurso porque tiene préstamos activos o vencidos pendientes de devolución.");
            }

            _recursoRepository.Delete(recurso);
            await _unitOfWork.SaveChangesAsync();
            
            InvalidateCache();
        }
    }
}
