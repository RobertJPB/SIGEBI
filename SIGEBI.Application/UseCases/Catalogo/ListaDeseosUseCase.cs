using SIGEBI.Business.DTOs;
using SIGEBI.Application.Interfaces;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Business.Mappers;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Entities.Recursos;

namespace SIGEBI.Business.UseCases.Catalogo
{
    // Gestiona la colección de recursos que el usuario desea leer o reservar a futuro.
    public class ListaDeseosUseCase
    {
        private readonly IListaDeseosRepository _listaDeseosRepository;
        private readonly IRecursoRepository _recursoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ListaDeseosUseCase(
            IListaDeseosRepository listaDeseosRepository,
            IRecursoRepository recursoRepository,
            IUsuarioRepository usuarioRepository,
            IUnitOfWork unitOfWork)
        {
            _listaDeseosRepository = listaDeseosRepository;
            _recursoRepository = recursoRepository;
            _usuarioRepository = usuarioRepository;
            _unitOfWork = unitOfWork;
        }

        // Obtiene la lista del usuario o crea una nueva si aún no la tiene.
        public async Task<ListaDeseosDTO> ObtenerPorUsuarioAsync(Guid usuarioId)
        {
            if (!await _usuarioRepository.ExistsAsync(usuarioId))
                throw new KeyNotFoundException("Usuario no encontrado.");

            var lista = await _listaDeseosRepository.GetByUsuarioIdAsync(usuarioId);
            
            // Si el usuario no tiene lista, le creamos una en blanco por defecto
            if (lista == null)
            {
                lista = new ListaDeseos(usuarioId, DateTime.UtcNow);
                await _listaDeseosRepository.AddAsync(lista);
                await _unitOfWork.SaveChangesAsync();
            }
            
            return ListaDeseosMapper.ToDTO(lista);
        }

        // Agrega una referencia de recurso a la lista de deseos del usuario.
        public async Task AgregarRecursoAsync(Guid usuarioId, Guid recursoId)
        {
            var recurso = await _recursoRepository.GetByIdAsync(recursoId)
                ?? throw new InvalidOperationException("Recurso no encontrado.");

            if (!await _usuarioRepository.ExistsAsync(usuarioId))
                throw new KeyNotFoundException("Usuario no encontrado.");

            var lista = await _listaDeseosRepository.GetByUsuarioIdAsync(usuarioId);
            if (lista == null)
            {
                lista = new ListaDeseos(usuarioId, DateTime.UtcNow);
                await _listaDeseosRepository.AddAsync(lista);
                // Guardamos primero la lista para asegurar que el registro padre exista (evita FK conflict)
                await _unitOfWork.SaveChangesAsync();
            }

            lista.AgregarRecurso(recurso);
            
            // No es necesario llamar a Update() si la entidad ya está siendo trackeada por EF.
            // Al llamar a SaveChangesAsync(), EF detectará el cambio en la colección Recursos.
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