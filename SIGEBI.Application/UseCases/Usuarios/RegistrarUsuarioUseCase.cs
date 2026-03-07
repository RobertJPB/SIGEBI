using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Business.Interfaces.Services;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Enums.Seguridad;

namespace SIGEBI.Business.UseCases.Usuarios
{
    public class RegistrarUsuarioUseCase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IHashService _hashService;

        public RegistrarUsuarioUseCase(
            IUsuarioRepository usuarioRepository,
            IHashService hashService)
        {
            _usuarioRepository = usuarioRepository;
            _hashService = hashService;
        }

        public async Task Ejecutar(UsuarioDTO dto)
        {
            var hash = _hashService.Hash(dto.Contrasena);

            var usuario = new Usuario(
                dto.Nombre,
                dto.Correo,
                hash,
                (RolUsuario)dto.IdRol
            );

            await _usuarioRepository.AddAsync(usuario);
        }
    }
}