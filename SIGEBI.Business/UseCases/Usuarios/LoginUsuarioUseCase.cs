using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.Interfaces.Services;
using SIGEBI.Business.Interfaces.UseCases.Usuarios;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.UseCases.Usuarios
{
    // Valida las credenciales de los usuarios comparando hashes de contraseñas.
    public class LoginUsuarioUseCase : ILoginUsuarioUseCase
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

            // La cuenta del sistema nunca puede iniciar sesión por el flujo normal.
            // Su hash es un centinela que ningún algoritmo de hashing real puede producir.
            if (usuario.ContrasenaHash == Usuario.SistemaHashCentinela)
                return null;

            // validamos la clave contra el hash guardado en BD
            var passwordValido = _hashService.Verificar(password, usuario.ContrasenaHash);
            if (!passwordValido)
                return null;

            return usuario;
        }
    }
}