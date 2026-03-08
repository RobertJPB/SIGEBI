using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Business.Mappers;

namespace SIGEBI.Business.UseCases.Catalogo
{
    public class ConsultarLibrosUseCase
    {
        private readonly IRecursoRepository _recursoRepository;
        private readonly IValoracionRepository _valoracionRepository;

        public ConsultarLibrosUseCase(
            IRecursoRepository recursoRepository,
            IValoracionRepository valoracionRepository)
        {
            _recursoRepository = recursoRepository;
            _valoracionRepository = valoracionRepository;
        }

        public async Task<IEnumerable<RecursoDetalleDTO>> EjecutarAsync()
        {
            var recursos = await _recursoRepository.GetDisponiblesAsync();
            var dtos = new List<RecursoDetalleDTO>();

            foreach (var recurso in recursos)
            {
                var dto = RecursoMapper.ToDTO(recurso);
                dto.PromedioValoraciones = await _valoracionRepository.GetPromedioCalificacionAsync(recurso.Id);
                dtos.Add(dto);
            }

            return dtos;
        }

        public async Task<IEnumerable<RecursoDetalleDTO>> BuscarPorTituloAsync(string titulo)
        {
            var recursos = await _recursoRepository.BuscarPorTituloAsync(titulo);
            return recursos.Select(RecursoMapper.ToDTO);
        }

        public async Task<IEnumerable<RecursoDetalleDTO>> BuscarPorCategoriaAsync(int categoriaId)
        {
            var recursos = await _recursoRepository.GetByCategoriaAsync(categoriaId);
            return recursos.Select(RecursoMapper.ToDTO);
        }
    }
}