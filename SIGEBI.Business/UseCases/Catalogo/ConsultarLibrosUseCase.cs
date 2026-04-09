using Microsoft.Extensions.Caching.Memory;
using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.Interfaces.UseCases.Catalogo;
using SIGEBI.Business.Mappers;

namespace SIGEBI.Business.UseCases.Catalogo
{
    // Permite buscar y listar recursos bibliográficos disponibles en el catálogo.
    public class ConsultarLibrosUseCase : IConsultarLibrosUseCase
    {
        private readonly IRecursoRepository _recursoRepository;
        private readonly IValoracionRepository _valoracionRepository;
        private readonly IMemoryCache _cache;

        public ConsultarLibrosUseCase(
            IRecursoRepository recursoRepository,
            IValoracionRepository valoracionRepository,
            IMemoryCache cache)
        {
            _recursoRepository = recursoRepository;
            _valoracionRepository = valoracionRepository;
            _cache = cache;
        }

        // Recupera todos los libros y documentos disponibles junto con su calificación promedio.
        public async Task<IEnumerable<RecursoDetalleDTO>> EjecutarAsync()
        {
            if (!_cache.TryGetValue("RecursosDisponibles", out IEnumerable<RecursoDetalleDTO>? dtos))
            {
                var recursos = await _recursoRepository.GetDisponiblesAsync();
                var recursoIds = recursos.Select(r => r.Id).ToList();
                
                // Traemos todos los promedios en una sola consulta batch (Optimizado)
                var promediosMap = await _valoracionRepository.GetPromediosBatchAsync(recursoIds);
                
                var list = new List<RecursoDetalleDTO>();

                foreach (var recurso in recursos)
                {
                    var dto = RecursoMapper.ToDTO(recurso);
                    // Buscamos el promedio ya cargado en el diccionario en memoria
                    dto.PromedioValoraciones = promediosMap.ContainsKey(recurso.Id) ? promediosMap[recurso.Id] : 0;
                    list.Add(dto);
                }
                dtos = list;
                _cache.Set("RecursosDisponibles", dtos, TimeSpan.FromMinutes(5));
            }

            return dtos!;
        }


        // Filtra los recursos por coincidencia parcial en el título.
        public async Task<IEnumerable<RecursoDetalleDTO>> BuscarPorTituloAsync(string titulo)
        {
            var recursos = await _recursoRepository.BuscarPorTituloAsync(titulo);
            return recursos.Select(RecursoMapper.ToDTO);
        }

        // Obtiene todos los recursos pertenecientes a una categoría específica.
        public async Task<IEnumerable<RecursoDetalleDTO>> BuscarPorCategoriaAsync(int categoriaId)
        {
            var recursos = await _recursoRepository.GetByCategoriaAsync(categoriaId);
            return recursos.Select(RecursoMapper.ToDTO);
        }

        public async Task<RecursoDetalleDTO?> GetByIdAsync(Guid id)
        {
            var recurso = await _recursoRepository.GetByIdAsync(id);
            if (recurso == null) return null;
            
            var dto = RecursoMapper.ToDTO(recurso);
            dto.PromedioValoraciones = await _valoracionRepository.GetPromedioCalificacionAsync(recurso.Id);
            return dto;
        }
    }
}
