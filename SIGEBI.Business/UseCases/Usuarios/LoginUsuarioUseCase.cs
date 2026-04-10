using SIGEBI.Business.Exceptions;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.Interfaces.Services;
using SIGEBI.Business.Interfaces.UseCases.Usuarios;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Enums.Seguridad;
using SIGEBI.Business.Interfaces.Common;
using SIGEBI.Domain.Enums.Auditoria;

namespace SIGEBI.Business.UseCases.Usuarios
{
    // Valida las credenciales de los usuarios comparando hashes de contraseñas.
    public class LoginUsuarioUseCase : ILoginUsuarioUseCase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IHashService _hashService;
        private readonly IAuditService _audit;

        public LoginUsuarioUseCase(
            IUsuarioRepository usuarioRepository,
            IHashService hashService,
            IAuditService audit)
        {
            _usuarioRepository = usuarioRepository;
            _hashService = hashService;
            _audit = audit;
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

            // Si las credenciales son válidas, verificamos el estado de la cuenta.
            // Solo los usuarios Activos pueden entrar al sistema.
            if (usuario.Estado != EstadoUsuario.Activo)
            {
                string? motivo = null;
                DateTime? fechaFin = null;

                if (usuario.Estado == EstadoUsuario.Suspendido)
                {
                    var penalizacion = usuario.Penalizaciones?.FirstOrDefault(p => p.Estado == SIGEBI.Domain.Enums.Operacion.EstadoPenalizacion.Activa);
                    motivo = penalizacion?.Motivo;
                    fechaFin = penalizacion?.FechaFin;
                }
                else
                {
                    motivo = usuario.MotivoEstado;
                }

                _audit.LogActionBackground(TipoAccionAuditoria.AccesoDenegado, "Seguridad", 
                    $"Acceso bloqueado para el usuario {usuario.Nombre}. Estado: {usuario.Estado}. Motivo: {motivo}", 
                    usuario.Id);

                throw new UsuarioEstadoException(usuario.Estado, motivo, fechaFin);
            }

            _audit.LogActionBackground(TipoAccionAuditoria.Login, "Seguridad", 
                $"Inicio de sesión exitoso para el usuario: {usuario.Nombre}", 
                usuario.Id);

            return usuario;
        }
    }
}
