using Microsoft.Extensions.Caching.Memory;
using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.Interfaces.UseCases.Catalogo;
using SIGEBI.Business.Mappers;
using SIGEBI.Domain.Entities;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SIGEBI.Business.UseCases.Catalogo
{
    // Controla todo el flujo de gestion de categorias de forma aislada.
    public class CategoriasUseCase : ICategoriasUseCase
    {
        // Solo inyectamos ICategoriaRepository. No le damos acceso a IUsuarioRepository ni a metodos que no necesita.
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;

        public CategoriasUseCase(
            ICategoriaRepository categoriaRepository,
            IUnitOfWork unitOfWork,
            IMemoryCache cache)
        {
            _categoriaRepository = categoriaRepository;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        private void InvalidateCache()
        {
            _cache.Remove("AllCategorias");
            _cache.Remove("ActiveCategorias");
        }

        // Recupera todas las categorías registradas para poblar selectores y filtros.
        public async Task<IEnumerable<CategoriaDTO>> ObtenerTodasAsync()
        {
            if (!_cache.TryGetValue("AllCategorias", out IEnumerable<CategoriaDTO>? categoriasDto))
            {
                var categorias = await _categoriaRepository.GetAllAsync();
                categoriasDto = categorias.Select(CategoriaMapper.ToDTO).ToList();
                _cache.Set("AllCategorias", categoriasDto, TimeSpan.FromMinutes(15));
            }
            return categoriasDto!;
        }

        public async Task<IEnumerable<CategoriaDTO>> ObtenerActivasAsync()
        {
            if (!_cache.TryGetValue("ActiveCategorias", out IEnumerable<CategoriaDTO>? categoriasDto))
            {
                var categorias = await _categoriaRepository.GetActivasAsync();
                categoriasDto = categorias.Select(CategoriaMapper.ToDTO).ToList();
                _cache.Set("ActiveCategorias", categoriasDto, TimeSpan.FromMinutes(15));
            }
            return categoriasDto!;
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
            
            InvalidateCache();

            return CategoriaMapper.ToDTO(categoria);
        }


        public async Task<CategoriaDTO> ActualizarAsync(int id, string nuevoNombre)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Categoria no encontrada.");

            categoria.CambiarNombre(nuevoNombre);
            
            _categoriaRepository.Update(categoria);
            await _unitOfWork.SaveChangesAsync();
            
            InvalidateCache();

            return CategoriaMapper.ToDTO(categoria);
        }

        public async Task EliminarAsync(int id)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Categoria no encontrada.");
            
            _categoriaRepository.Delete(categoria);
            await _unitOfWork.SaveChangesAsync();
            
            InvalidateCache();
        }
    }
}
