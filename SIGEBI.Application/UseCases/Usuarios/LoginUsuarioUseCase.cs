using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Business.Interfaces.Services;
using SIGEBI.Business.Mappers;

namespace SIGEBI.Business.UseCases.Usuarios
{
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

        public async Task<UsuarioDTO?> Ejecutar(string correo, string password)
        {
            var usuario = await _usuarioRepository.GetByCorreoAsync(correo);

            if (usuario == null)
                return null;

            var passwordValido = _hashService.Verificar(password, usuario.ContrasenaHash);

            if (!passwordValido)
                return null;

            return UsuarioMapper.ToDTO(usuario);
        }
    }
}