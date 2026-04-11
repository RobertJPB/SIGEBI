using Microsoft.Extensions.Caching.Memory;
using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.Mappers;
using SIGEBI.Business.Interfaces.Common;
using SIGEBI.Business.Interfaces.UseCases.Catalogo;
using SIGEBI.Domain.Entities.Recursos;
using SIGEBI.Domain.ValueObjects;
using SIGEBI.Business.Interfaces.Services;

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
        private readonly IImagenService _imagenService;

        public GestionarRecursosUseCase(
            IRecursoRepository recursoRepository,
            IPrestamoRepository prestamoRepository,
            ICategoriaRepository categoriaRepository,
            IUnitOfWork unitOfWork,
            IMemoryCache cache,
            IGuidGenerator guidGenerator,
            IImagenService imagenService)
        {
            _recursoRepository = recursoRepository;
            _prestamoRepository = prestamoRepository;
            _categoriaRepository = categoriaRepository;
            _unitOfWork = unitOfWork;
            _cache = cache;
            _guidGenerator = guidGenerator;
            _imagenService = imagenService;
        }

        private void InvalidateCache()
        {
            _cache.Remove("RecursosDisponibles");
        }

        // ── AGREGAR ──

        public async Task<RecursoDetalleDTO> AgregarLibroAsync(LibroRequestDTO request)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(request.CategoriaId)
                ?? throw new InvalidOperationException("Categoría no encontrada.");
            
            string? imagenUrl = await _imagenService.GuardarImagenAsync(request.ImagenStream, request.ImagenNombre ?? "portada.jpg", "recursos");

            var libro = new Libro(_guidGenerator.Create(), request.Titulo, request.Autor, request.CategoriaId, request.Stock, request.Descripcion, new ISBN(request.ISBN), request.Editorial, request.Anio, request.Genero);
            if (imagenUrl != null) libro.ActualizarImagen(imagenUrl);
            
            await _recursoRepository.AddAsync(libro);
            await _unitOfWork.SaveChangesAsync();
            
            InvalidateCache();
            return RecursoMapper.ToDTO(libro);
        }

        public async Task<RecursoDetalleDTO> AgregarRevistaAsync(RevistaRequestDTO request)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(request.CategoriaId)
                ?? throw new InvalidOperationException("Categoría no encontrada.");

            string? imagenUrl = await _imagenService.GuardarImagenAsync(request.ImagenStream, request.ImagenNombre ?? "portada.jpg", "recursos");

            var revista = new Revista(_guidGenerator.Create(), request.Titulo, request.Autor, request.CategoriaId, request.Stock, request.Descripcion, request.NumeroEdicion, request.ISSN, request.Anio, request.Editorial);
            if (imagenUrl != null) revista.ActualizarImagen(imagenUrl);
            
            await _recursoRepository.AddAsync(revista);
            await _unitOfWork.SaveChangesAsync();
            
            InvalidateCache();
            return RecursoMapper.ToDTO(revista);
        }

        public async Task<RecursoDetalleDTO> AgregarDocumentoAsync(DocumentoRequestDTO request)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(request.CategoriaId)
                ?? throw new InvalidOperationException("Categoría no encontrada.");

            string? imagenUrl = await _imagenService.GuardarImagenAsync(request.ImagenStream, request.ImagenNombre ?? "portada.jpg", "recursos");

            var documento = new Documento(_guidGenerator.Create(), request.Titulo, request.Autor, request.CategoriaId, request.Stock, request.Descripcion, request.Formato, request.Institucion, request.Anio);
            if (imagenUrl != null) documento.ActualizarImagen(imagenUrl);
            
            await _recursoRepository.AddAsync(documento);
            await _unitOfWork.SaveChangesAsync();
            
            InvalidateCache();
            return RecursoMapper.ToDTO(documento);
        }

        // ── EDITAR ──

        public async Task<RecursoDetalleDTO> EditarLibroAsync(Guid id, LibroRequestDTO request)
        {
            var libro = await _recursoRepository.GetByIdAsync(id) as Libro
                ?? throw new KeyNotFoundException("Libro no encontrado.");
            var categoria = await _categoriaRepository.GetByIdAsync(request.CategoriaId)
                ?? throw new InvalidOperationException("Categoría no encontrada.");

            string? imagenUrl = await _imagenService.GuardarImagenAsync(request.ImagenStream, request.ImagenNombre ?? "portada.jpg", "recursos");

            libro.Actualizar(request.Titulo, request.Autor, request.CategoriaId, request.Stock, request.Descripcion, new ISBN(request.ISBN), request.Editorial, request.Anio, request.Genero);
            if (imagenUrl != null) {
                _imagenService.EliminarImagen(libro.ImagenUrl);
                libro.ActualizarImagen(imagenUrl);
            }

            _recursoRepository.Update(libro);
            await _unitOfWork.SaveChangesAsync();
            
            InvalidateCache();
            return RecursoMapper.ToDTO(libro);
        }

        public async Task<RecursoDetalleDTO> EditarRevistaAsync(Guid id, RevistaRequestDTO request)
        {
            var revista = await _recursoRepository.GetByIdAsync(id) as Revista
                ?? throw new InvalidOperationException("Revista no encontrada.");
            var categoria = await _categoriaRepository.GetByIdAsync(request.CategoriaId)
                ?? throw new InvalidOperationException("Categoría no encontrada.");

            string? imagenUrl = await _imagenService.GuardarImagenAsync(request.ImagenStream, request.ImagenNombre ?? "portada.jpg", "recursos");

            revista.Actualizar(request.Titulo, request.Autor, request.CategoriaId, request.Stock, request.Descripcion, request.NumeroEdicion, request.ISSN, request.Anio, request.Editorial);
            if (imagenUrl != null) {
                _imagenService.EliminarImagen(revista.ImagenUrl);
                revista.ActualizarImagen(imagenUrl);
            }

            _recursoRepository.Update(revista);
            await _unitOfWork.SaveChangesAsync();
            
            InvalidateCache();
            return RecursoMapper.ToDTO(revista);
        }

        public async Task<RecursoDetalleDTO> EditarDocumentoAsync(Guid id, DocumentoRequestDTO request)
        {
            var documento = await _recursoRepository.GetByIdAsync(id) as Documento
                ?? throw new KeyNotFoundException("Documento no encontrado.");
            var categoria = await _categoriaRepository.GetByIdAsync(request.CategoriaId)
                ?? throw new InvalidOperationException("Categoría no encontrada.");

            string? imagenUrl = await _imagenService.GuardarImagenAsync(request.ImagenStream, request.ImagenNombre ?? "portada.jpg", "recursos");

            documento.Actualizar(request.Titulo, request.Autor, request.CategoriaId, request.Stock, request.Descripcion, request.Formato, request.Institucion, request.Anio);
            if (imagenUrl != null) {
                _imagenService.EliminarImagen(documento.ImagenUrl);
                documento.ActualizarImagen(imagenUrl);
            }

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

            _imagenService.EliminarImagen(recurso.ImagenUrl);
            _recursoRepository.Delete(recurso);
            await _unitOfWork.SaveChangesAsync();
            
            InvalidateCache();
        }
    }
}
