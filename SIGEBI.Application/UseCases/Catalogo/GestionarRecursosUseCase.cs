using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Business.Mappers;
using SIGEBI.Domain.Entities.Recursos;

namespace SIGEBI.Business.UseCases.Catalogo
{
    public class GestionarRecursosUseCase
    {
        private readonly IRecursoRepository _recursoRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public GestionarRecursosUseCase(
            IRecursoRepository recursoRepository,
            ICategoriaRepository categoriaRepository,
            IUnitOfWork unitOfWork)
        {
            _recursoRepository = recursoRepository;
            _categoriaRepository = categoriaRepository;
            _unitOfWork = unitOfWork;
        }

        // ── AGREGAR ──

        public async Task<RecursoDetalleDTO> AgregarLibroAsync(string titulo, string autor,
            int categoriaId, int stock, string isbn, string editorial, int anio,
            string? imagenUrl = null, string? genero = null)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(categoriaId)
                ?? throw new InvalidOperationException("Categoría no encontrada.");
            var libro = new Libro(titulo, autor, categoriaId, stock, isbn, editorial, anio, genero);
            if (imagenUrl != null) libro.ActualizarImagen(imagenUrl);
            await _recursoRepository.AddAsync(libro);
            await _unitOfWork.SaveChangesAsync();
            return RecursoMapper.ToDTO(libro);
        }

        public async Task<RecursoDetalleDTO> AgregarRevistaAsync(string titulo, string autor,
            int categoriaId, int stock, int numeroEdicion, string issn, DateTime fechaPublicacion,
            string? imagenUrl = null)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(categoriaId)
                ?? throw new InvalidOperationException("Categoría no encontrada.");
            var revista = new Revista(titulo, autor, categoriaId, stock, numeroEdicion, issn, fechaPublicacion);
            if (imagenUrl != null) revista.ActualizarImagen(imagenUrl);
            await _recursoRepository.AddAsync(revista);
            await _unitOfWork.SaveChangesAsync();
            return RecursoMapper.ToDTO(revista);
        }

        public async Task<RecursoDetalleDTO> AgregarDocumentoAsync(string titulo, string autor,
            int categoriaId, int stock, string formato, string institucion, int anio,
            string? imagenUrl = null)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(categoriaId)
                ?? throw new InvalidOperationException("Categoría no encontrada.");
            var documento = new Documento(titulo, autor, categoriaId, stock, formato, institucion, anio);
            if (imagenUrl != null) documento.ActualizarImagen(imagenUrl);
            await _recursoRepository.AddAsync(documento);
            await _unitOfWork.SaveChangesAsync();
            return RecursoMapper.ToDTO(documento);
        }

        // ── EDITAR ──

        public async Task<RecursoDetalleDTO> EditarLibroAsync(Guid id, string titulo, string autor,
            int categoriaId, int stock, string isbn, string editorial, int anio,
            string? imagenUrl = null, string? genero = null)
        {
            var libro = await _recursoRepository.GetByIdAsync(id) as Libro
                ?? throw new InvalidOperationException("Libro no encontrado.");
            var categoria = await _categoriaRepository.GetByIdAsync(categoriaId)
                ?? throw new InvalidOperationException("Categoría no encontrada.");
            libro.Actualizar(titulo, autor, categoriaId, stock, isbn, editorial, anio, genero);
            if (imagenUrl != null) libro.ActualizarImagen(imagenUrl);
            _recursoRepository.Update(libro);
            await _unitOfWork.SaveChangesAsync();
            return RecursoMapper.ToDTO(libro);
        }

        public async Task<RecursoDetalleDTO> EditarRevistaAsync(Guid id, string titulo, string autor,
            int categoriaId, int stock, int numeroEdicion, string issn, DateTime fechaPublicacion,
            string? imagenUrl = null)
        {
            var revista = await _recursoRepository.GetByIdAsync(id) as Revista
                ?? throw new InvalidOperationException("Revista no encontrada.");
            var categoria = await _categoriaRepository.GetByIdAsync(categoriaId)
                ?? throw new InvalidOperationException("Categoría no encontrada.");
            revista.Actualizar(titulo, autor, categoriaId, stock, numeroEdicion, issn, fechaPublicacion);
            if (imagenUrl != null) revista.ActualizarImagen(imagenUrl);
            _recursoRepository.Update(revista);
            await _unitOfWork.SaveChangesAsync();
            return RecursoMapper.ToDTO(revista);
        }

        public async Task<RecursoDetalleDTO> EditarDocumentoAsync(Guid id, string titulo, string autor,
            int categoriaId, int stock, string formato, string institucion, int anio,
            string? imagenUrl = null)
        {
            var documento = await _recursoRepository.GetByIdAsync(id) as Documento
                ?? throw new InvalidOperationException("Documento no encontrado.");
            var categoria = await _categoriaRepository.GetByIdAsync(categoriaId)
                ?? throw new InvalidOperationException("Categoría no encontrada.");
            documento.Actualizar(titulo, autor, categoriaId, stock, formato, institucion, anio);
            if (imagenUrl != null) documento.ActualizarImagen(imagenUrl);
            _recursoRepository.Update(documento);
            await _unitOfWork.SaveChangesAsync();
            return RecursoMapper.ToDTO(documento);
        }

  

        public async Task ActualizarImagenAsync(Guid recursoId, string imagenUrl)
        {
            var recurso = await _recursoRepository.GetByIdAsync(recursoId)
                ?? throw new InvalidOperationException("Recurso no encontrado.");
            recurso.ActualizarImagen(imagenUrl);
            _recursoRepository.Update(recurso);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task EliminarRecursoAsync(Guid recursoId)
        {
            var recurso = await _recursoRepository.GetByIdAsync(recursoId)
                ?? throw new InvalidOperationException("Recurso no encontrado.");
            recurso.Desactivar();
            _recursoRepository.Update(recurso);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
