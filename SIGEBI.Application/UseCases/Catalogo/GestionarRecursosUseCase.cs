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

        public async Task<RecursoDetalleDTO> AgregarLibroAsync(string titulo, string autor,
            int categoriaId, int stock, string isbn, string editorial, int anio)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(categoriaId)
                ?? throw new InvalidOperationException("Categoría no encontrada.");

            var libro = new Libro(titulo, autor, categoriaId, stock, isbn, editorial, anio);
            await _recursoRepository.AddAsync(libro);
            await _unitOfWork.SaveChangesAsync();
            return RecursoMapper.ToDTO(libro);
        }

        public async Task<RecursoDetalleDTO> AgregarRevistaAsync(string titulo, string autor,
            int categoriaId, int stock, int numeroEdicion, string issn, DateTime fechaPublicacion)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(categoriaId)
                ?? throw new InvalidOperationException("Categoría no encontrada.");

            var revista = new Revista(titulo, autor, categoriaId, stock, numeroEdicion, issn, fechaPublicacion);
            await _recursoRepository.AddAsync(revista);
            await _unitOfWork.SaveChangesAsync();
            return RecursoMapper.ToDTO(revista);
        }

        public async Task<RecursoDetalleDTO> AgregarDocumentoAsync(string titulo, string autor,
            int categoriaId, int stock, string formato, string institucion, int anio)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(categoriaId)
                ?? throw new InvalidOperationException("Categoría no encontrada.");

            var documento = new Documento(titulo, autor, categoriaId, stock, formato, institucion, anio);
            await _recursoRepository.AddAsync(documento);
            await _unitOfWork.SaveChangesAsync();
            return RecursoMapper.ToDTO(documento);
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