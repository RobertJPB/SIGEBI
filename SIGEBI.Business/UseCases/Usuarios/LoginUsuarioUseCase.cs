using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.Interfaces.Services;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.UseCases.Usuarios
{
    // Valida las credenciales de los usuarios comparando hashes de contraseñas.
    public class LoginUsuarioUseCase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IHashService _hashService;

        public LoginUsuarioUseCase(
            IUsuarioRepository usuarioRepository,
            IHashService hashService)
        {
            _usuarioRepository = usuarioRepository;
            _hashService = hashService;
        }

        // Verifica correo y clave, retornando la entidad de usuario si la validación es exitosa.
        public async Task<Usuario?> EjecutarAsync(string correo, string password)
        {
            var usuario = await _usuarioRepository.GetByCorreoAsync(correo);
            if (usuario == null)
                return null; // si no existe, devolvemos null para que el controller tire error

            // validamos la clave contra el hash guardado en BD
            var passwordValido = _hashService.Verificar(password, usuario.ContrasenaHash);
            if (!passwordValido)
                return null;

            return usuario;
        }
    }
}