using SIGEBI.Business.DTOs;
using SIGEBI.Application.Interfaces;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Business.Mappers;
using SIGEBI.Domain.Enums.Seguridad;

namespace SIGEBI.Business.UseCases.Usuarios
{
    public class GestionarUsuarioUseCase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUnitOfWork _unitOfWork;

        public GestionarUsuarioUseCase(
            IUsuarioRepository usuarioRepository,
            IUnitOfWork unitOfWork)
        {
            _usuarioRepository = usuarioRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<UsuarioDTO>> ObtenerTodosAsync()
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            return usuarios.Select(UsuarioMapper.ToDTO);
        }

        public async Task<UsuarioDTO?> ObtenerPorIdAsync(Guid id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            return usuario == null ? null : UsuarioMapper.ToDTO(usuario);
        }

        public async Task ActivarAsync(Guid id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Usuario no encontrado.");
            usuario.Activar();
            _usuarioRepository.Update(usuario);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DesactivarAsync(Guid id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Usuario no encontrado.");
            usuario.Desactivar();
            _usuarioRepository.Update(usuario);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task BloquearAsync(Guid id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Usuario no encontrado.");
            usuario.Bloquear();
            _usuarioRepository.Update(usuario);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CambiarRolAsync(Guid id, RolUsuario nuevoRol)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Usuario no encontrado.");
            usuario.CambiarRol(nuevoRol);
            _usuarioRepository.Update(usuario);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}