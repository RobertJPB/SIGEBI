using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.Interfaces.Services;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Enums.Seguridad;

namespace SIGEBI.Business.UseCases.Usuarios
{
    // Encargado de dar de alta nuevos usuarios asegurando la seguridad de sus credenciales.
    public class RegistrarUsuarioUseCase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IHashService _hashService;
        private readonly IUnitOfWork _unitOfWork;

        public RegistrarUsuarioUseCase(
            IUsuarioRepository usuarioRepository,
            IHashService hashService,
            IUnitOfWork unitOfWork)
        {
            _usuarioRepository = usuarioRepository;
            _hashService = hashService;
            _unitOfWork = unitOfWork;
        }

        // Crea un nuevo usuario encriptando su clave y validando que el correo sea único.
        public async Task<Guid> EjecutarAsync(UsuarioDTO dto)
        {
            // Ojo: validar que el correo no este repetido
            var existente = await _usuarioRepository.GetByCorreoAsync(dto.Correo);
            if (existente != null)
                throw new InvalidOperationException("Ya existe un usuario con ese correo.");

            // encriptamos la clave antes de guardarla
            var hash = _hashService.Hash(dto.Contrasena);

            var usuario = new Usuario(
                dto.Nombre,
                dto.Correo,
                hash,
                (RolUsuario)dto.IdRol
            );

            await _usuarioRepository.AddAsync(usuario);
            await _unitOfWork.SaveChangesAsync(); // ← Este era el problema (ya arreglado)

            return usuario.Id;
        }
    }
}