using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.Interfaces.UseCases.Catalogo;
using SIGEBI.Business.Mappers;
using SIGEBI.Business.Interfaces.Common;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Entities.Recursos;

namespace SIGEBI.Business.UseCases.Catalogo
{
    // Gestiona la colección de recursos que el usuario desea leer o reservar a futuro.
    public class ListaDeseosUseCase : IListaDeseosUseCase
    {
        private readonly IListaDeseosRepository _listaDeseosRepository;
        private readonly IRecursoRepository _recursoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGuidGenerator _guidGenerator;

        public ListaDeseosUseCase(
            IListaDeseosRepository listaDeseosRepository,
            IRecursoRepository recursoRepository,
            IUsuarioRepository usuarioRepository,
            IUnitOfWork unitOfWork,
            IGuidGenerator guidGenerator)
        {
            _listaDeseosRepository = listaDeseosRepository;
            _recursoRepository = recursoRepository;
            _usuarioRepository = usuarioRepository;
            _unitOfWork = unitOfWork;
            _guidGenerator = guidGenerator;
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
                lista = new ListaDeseos(_guidGenerator.Create(), usuarioId, DateTime.UtcNow);
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
                lista = new ListaDeseos(_guidGenerator.Create(), usuarioId, DateTime.UtcNow);
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

            // Verificamos si el recurso existe antes de intentar removerlo para dar un error claro si no esta
            if (!lista.Recursos.Any(r => r.Id == recursoId))
                throw new InvalidOperationException("El recurso no se encuentra en la lista de deseos.");

            lista.RemoverRecurso(recursoId);
            
            // IMPORTANTE: No llamamos a _listaDeseosRepository.Update(lista) porque la entidad 
            // ya esta siendo trackeada por EF al venir del repositorio con Include().
            // Llamar a Update() puede interferir con la deteccion de cambios en colecciones many-to-many.
            
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
