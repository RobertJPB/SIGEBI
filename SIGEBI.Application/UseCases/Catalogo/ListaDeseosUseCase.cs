using SIGEBI.Business.DTOs;
using SIGEBI.Application.Interfaces;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Business.Mappers;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Entities.Recursos;

namespace SIGEBI.Business.UseCases.Catalogo
{
    public class ListaDeseosUseCase
    {
        private readonly IListaDeseosRepository _listaDeseosRepository;
        private readonly IRecursoRepository _recursoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ListaDeseosUseCase(
            IListaDeseosRepository listaDeseosRepository,
            IRecursoRepository recursoRepository,
            IUnitOfWork unitOfWork)
        {
            _listaDeseosRepository = listaDeseosRepository;
            _recursoRepository = recursoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ListaDeseosDTO> ObtenerPorUsuarioAsync(Guid usuarioId)
        {
            var lista = await _listaDeseosRepository.GetByUsuarioIdAsync(usuarioId);
            if (lista == null)
            {
                lista = new ListaDeseos(usuarioId, DateTime.UtcNow);
                await _listaDeseosRepository.AddAsync(lista);
                await _unitOfWork.SaveChangesAsync();
            }
            return ListaDeseosMapper.ToDTO(lista);
        }

        public async Task AgregarRecursoAsync(Guid usuarioId, Guid recursoId)
        {
            var recurso = await _recursoRepository.GetByIdAsync(recursoId)
                ?? throw new InvalidOperationException("Recurso no encontrado.");

            var lista = await _listaDeseosRepository.GetByUsuarioIdAsync(usuarioId);
            if (lista == null)
            {
                lista = new ListaDeseos(usuarioId, DateTime.UtcNow);
                await _listaDeseosRepository.AddAsync(lista);
            }

            lista.AgregarRecurso(recurso);
            _listaDeseosRepository.Update(lista);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoverRecursoAsync(Guid usuarioId, Guid recursoId)
        {
            var lista = await _listaDeseosRepository.GetByUsuarioIdAsync(usuarioId)
                ?? throw new InvalidOperationException("Lista de deseos no encontrada.");

            lista.RemoverRecurso(recursoId);
            _listaDeseosRepository.Update(lista);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}