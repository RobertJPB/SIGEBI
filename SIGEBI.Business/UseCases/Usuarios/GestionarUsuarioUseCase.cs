using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.Interfaces.UseCases.Usuarios;
using SIGEBI.Business.Mappers;
using SIGEBI.Domain.Enums.Seguridad;
using SIGEBI.Domain.ValueObjects;
using Microsoft.Extensions.Caching.Memory;

namespace SIGEBI.Business.UseCases.Usuarios
{
    // Administra el ciclo de vida y los permisos de los usuarios registrados.
    public class GestionarUsuarioUseCase : IGestionarUsuarioUseCase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly INotificacionesUseCase _notificaciones;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;
        private const string CachePrefix = "UserStatus_";

        public GestionarUsuarioUseCase(
            IUsuarioRepository usuarioRepository,
            INotificacionesUseCase notificaciones,
            IUnitOfWork unitOfWork,
            IMemoryCache cache)
        {
            _usuarioRepository = usuarioRepository;
            _notificaciones = notificaciones;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<IEnumerable<UsuarioDTO>> ObtenerTodosAsync()
        {
            var usuarios = await _usuarioRepository.GetAllConPenalizacionesAsync();
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
            
            _cache.Remove($"{CachePrefix}{id}");
        }

        public async Task DesactivarAsync(Guid id, string motivo)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Usuario no encontrado.");
            usuario.Desactivar(motivo);
            _usuarioRepository.Update(usuario);
            _cache.Remove($"{CachePrefix}{usuario.Id}");

            // Notificar al usuario (aunque no pueda loguearse, queda el registro)
            await _notificaciones.EnviarNotificacionAsync(id, $"Su cuenta ha sido desactivada administrativamente. Motivo: {motivo}");

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task SuspenderAsync(Guid id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Usuario no encontrado.");
            usuario.Suspender();
            _usuarioRepository.Update(usuario);
            await _unitOfWork.SaveChangesAsync();
            
            _cache.Remove($"{CachePrefix}{id}");
        }

        // Restringe permanentemente el acceso de un usuario al sistema.
        public async Task BloquearAsync(Guid id, string motivo)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Usuario no encontrado.");
            
            usuario.Bloquear(motivo);
            _usuarioRepository.Update(usuario);
            _cache.Remove($"{CachePrefix}{usuario.Id}");

            // Notificar al usuario
            await _notificaciones.EnviarNotificacionAsync(id, $"Su cuenta ha sido bloqueada permanentemente por faltas graves. Motivo: {motivo}");

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
            
            // Usamos HardDelete para asegurar el borrado físico de la base de datos
            _usuarioRepository.HardDelete(usuario);
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
            usuario.CambiarCorreo(new Email(nuevoCorreo));
            
            _usuarioRepository.Update(usuario);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
