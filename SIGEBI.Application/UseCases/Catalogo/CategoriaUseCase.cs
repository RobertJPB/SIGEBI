using SIGEBI.Business.DTOs;
using SIGEBI.Application.Interfaces;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Business.Mappers;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.UseCases.Catalogo
{
    // Controla todo el flujo de gestion de categorias de forma aislada.
    public class CategoriasUseCase
    {
        // Solo inyectamos ICategoriaRepository. No le damos acceso a IUsuarioRepository ni a metodos que no necesita.
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CategoriasUseCase(
            ICategoriaRepository categoriaRepository,
            IUnitOfWork unitOfWork)
        {
            _categoriaRepository = categoriaRepository;
            _unitOfWork = unitOfWork;
        }

        // Recupera todas las categorías registradas para poblar selectores y filtros.
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

        // Crea una nueva categoría validando que el nombre no sea duplicado.
        public async Task<CategoriaDTO> CrearAsync(string nombre)
        {
            var existe = await _categoriaRepository.GetByNombreAsync(nombre);
            if (existe != null)
                throw new InvalidOperationException("Ya existe una categoria con ese nombre.");

            var categoria = new Categoria(nombre);
            
            // Console.WriteLine("Guardando categoria en BD...");
            await _categoriaRepository.AddAsync(categoria);
            await _unitOfWork.SaveChangesAsync();
            
            return CategoriaMapper.ToDTO(categoria);
        }

        // Habilita una categoría para que pueda ser asignada a nuevos recursos.
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
