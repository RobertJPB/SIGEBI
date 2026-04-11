using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.Interfaces.Services;
using SIGEBI.Business.Interfaces.Common;
using SIGEBI.Business.Interfaces.UseCases.Usuarios;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Enums.Seguridad;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Business.UseCases.Usuarios
{
    // Encargado de dar de alta nuevos usuarios asegurando la seguridad de sus credenciales.
    public class RegistrarUsuarioUseCase : IRegistrarUsuarioUseCase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IHashService _hashService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IEmailAdapter _emailAdapter;

        public RegistrarUsuarioUseCase(
            IUsuarioRepository usuarioRepository,
            IHashService hashService,
            IUnitOfWork unitOfWork,
            IGuidGenerator guidGenerator,
            IEmailAdapter emailAdapter)
        {
            _usuarioRepository = usuarioRepository;
            _hashService = hashService;
            _unitOfWork = unitOfWork;
            _guidGenerator = guidGenerator;
            _emailAdapter = emailAdapter;
        }

        // Crea un nuevo usuario encriptando su clave.
        public async Task<Guid> EjecutarAsync(UsuarioDTO dto)
        {
            // chequear correo otra vez por seguridad
            var emailVo = new Email(dto.Correo);
            var existente = await _usuarioRepository.GetByCorreoAsync(emailVo.Value);
            if (existente != null)
                throw new InvalidOperationException("Ya existe un usuario con ese correo.");

            // validar formato antes de hashear
            var contrasenaVo = new Contrasena(dto.Contrasena);
            var hash = _hashService.Hash(contrasenaVo.Value);

            var usuario = new Usuario(
                _guidGenerator.Create(),
                dto.Nombre,
                emailVo,
                hash,
                (RolUsuario)dto.IdRol
            );

            if (!string.IsNullOrEmpty(dto.ImagenUrl))
            {
                usuario.ActualizarImagen(dto.ImagenUrl);
            }

            await _usuarioRepository.AddAsync(usuario);
            await _unitOfWork.SaveChangesAsync();

            // Enviar correo de bienvenida en segundo plano
            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailAdapter.EnviarAsync(usuario.Correo.Value, "Bienvenido a SIGEBI",
                        $"Hola {usuario.Nombre}, ¡bienvenido al Sistema de Gestión Bibliotecaria (SIGEBI)! " +
                        "Ya puedes acceder al catálogo y solicitar préstamos.");
                }
                catch
                {
                    // No bloqueamos el registro si el email falla
                }
            });

            return usuario.Id;
        }
    }
}
