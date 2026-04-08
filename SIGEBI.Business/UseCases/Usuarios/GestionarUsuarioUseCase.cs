using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.Interfaces.UseCases.Usuarios;
using SIGEBI.Business.Mappers;
using SIGEBI.Domain.Enums.Seguridad;

namespace SIGEBI.Business.UseCases.Usuarios
{
    // Administra el ciclo de vida y los permisos de los usuarios registrados.
    public class GestionarUsuarioUseCase : IGestionarUsuarioUseCase
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

        // Rehabilita el acceso al sistema para un usuario previamente desactivado.
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

        // Restringe permanentemente el acceso de un usuario al sistema.
        public async Task BloquearAsync(Guid id)
        {
            // TODO: Agregar motivo de bloqueo despues?
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

        // Elimina permanentemente un usuario del sistema (hard delete).
        public async Task EliminarAsync(Guid id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Usuario no encontrado.");
            _usuarioRepository.Delete(usuario);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ActualizarImagenAsync(Guid id, string? nuevaUrl)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Usuario no encontrado.");
            usuario.ActualizarImagen(nuevaUrl);
            _usuarioRepository.Update(usuario);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ActualizarPerfilAsync(Guid id, string nuevoNombre, string nuevoCorreo)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Usuario no encontrado.");
            
            usuario.CambiarNombre(nuevoNombre);
            usuario.CambiarCorreo(nuevoCorreo);
            
            _usuarioRepository.Update(usuario);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
