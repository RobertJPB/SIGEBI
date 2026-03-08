using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Business.Mappers;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.UseCases.Catalogo
{
    public class CategoriasUseCase
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CategoriasUseCase(
            ICategoriaRepository categoriaRepository,
            IUnitOfWork unitOfWork)
        {
            _categoriaRepository = categoriaRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CategoriaDTO>> ObtenerTodasAsync()
        {
            var categorias = await _categoriaRepository.GetAllAsync();
            return categorias.Select(CategoriaMapper.ToDTO);
        }

        public async Task<CategoriaDTO> ObtenerPorIdAsync(int id)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Categoria no encontrada.");
            return CategoriaMapper.ToDTO(categoria);
        }

        public async Task<CategoriaDTO> CrearAsync(string nombre)
        {
            var existe = await _categoriaRepository.GetByNombreAsync(nombre);
            if (existe != null)
                throw new InvalidOperationException("Ya existe una categoria con ese nombre.");

            var categoria = new Categoria(nombre);
            await _categoriaRepository.AddAsync(categoria);
            await _unitOfWork.SaveChangesAsync();
            return CategoriaMapper.ToDTO(categoria);
        }

        public async Task ActivarAsync(int id)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Categoria no encontrada.");
            categoria.Activar();
            _categoriaRepository.Update(categoria);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DesactivarAsync(int id)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Categoria no encontrada.");
            categoria.Desactivar();
            _categoriaRepository.Update(categoria);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}