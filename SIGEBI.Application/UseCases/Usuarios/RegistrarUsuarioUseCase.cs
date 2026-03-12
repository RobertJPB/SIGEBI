using SIGEBI.Business.DTOs;
using SIGEBI.Application.Interfaces;
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

        public async Task EjecutarAsync(UsuarioDTO dto)
        {
            var existente = await _usuarioRepository.GetByCorreoAsync(dto.Correo);
            if (existente != null)
                throw new InvalidOperationException("Ya existe un usuario con ese correo.");

            var hash = _hashService.Hash(dto.Contrasena);

            var usuario = new Usuario(
                dto.Nombre,
                dto.Correo,
                hash,
                (RolUsuario)dto.IdRol
            );

            await _usuarioRepository.AddAsync(usuario);
            await _unitOfWork.SaveChangesAsync(); // ← Este era el problema
        }
    }
}