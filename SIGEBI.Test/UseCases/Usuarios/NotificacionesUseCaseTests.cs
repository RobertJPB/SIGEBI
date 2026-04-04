using FluentAssertions;
using Moq;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.UseCases.Usuarios;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Enums.Operacion;
using SIGEBI.Domain.Enums.Seguridad;
using SIGEBI.Business.Interfaces.Common;
using Xunit;

namespace SIGEBI.Test.UseCases.Usuarios
{
    public class NotificacionesUseCaseTests
    {
        private readonly Mock<INotificacionRepository> _notificacionRepo;
        private readonly Mock<IUsuarioRepository> _usuarioRepo;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly NotificacionesUseCase _useCase;

        public NotificacionesUseCaseTests()
        {
            _notificacionRepo = new Mock<INotificacionRepository>();
            _usuarioRepo = new Mock<IUsuarioRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();

            _useCase = new NotificacionesUseCase(
                _notificacionRepo.Object,
                _usuarioRepo.Object,
                _unitOfWork.Object,
                new Mock<IGuidGenerator>().Object);
        }

        // Ã¢â€â‚¬Ã¢â€â‚¬ OBTENER NOTIFICACIONES Ã¢â€â‚¬Ã¢â€â‚¬

        // Caso de Uso: Notificaciones - Proceso: Consultar todas las alertas enviadas a un usuario particular.
        [Fact]
        public async Task ObtenerPorUsuario_UsuarioExistente_DevuelveLista()
        {
            // Arrange
            var usuario = new Usuario(Guid.NewGuid(), "T", "t@t.com", "h", RolUsuario.Estudiante);
            var notificaciones = new List<Notificacion>
            {
                new Notificacion(Guid.NewGuid(), usuario.Id, TipoNotificacion.Recordatorio, "Mensaje", DateTime.UtcNow)
            };
            _usuarioRepo.Setup(r => r.GetByIdAsync(usuario.Id)).ReturnsAsync(usuario);
            _notificacionRepo.Setup(r => r.GetByUsuarioIdAsync(usuario.Id)).ReturnsAsync(notificaciones);

            // Act
            var resultado = await _useCase.ObtenerPorUsuarioAsync(usuario.Id);

            // Assert
            resultado.Should().HaveCount(1);
        }

        // Ã¢â€â‚¬Ã¢â€â‚¬ MARCAR COMO LEIDA Ã¢â€â‚¬Ã¢â€â‚¬

        // Caso de Uso: Notificaciones - Proceso: Cambiar el estado de una alerta a "LeÃƒÂ­da".
        [Fact]
        public async Task MarcarComoLeida_Existente_CambiaEstadoAnLeida()
        {
            // Arrange
            var notificacion = new Notificacion(Guid.NewGuid(), Guid.NewGuid(), TipoNotificacion.Recordatorio, "M", DateTime.UtcNow);
            _notificacionRepo.Setup(r => r.GetByIdAsync(notificacion.Id)).ReturnsAsync(notificacion);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _useCase.MarcarComoLeidaAsync(notificacion.Id);

            // Assert
            notificacion.Estado.Should().Be(EstadoNotificacion.Leida);
            _notificacionRepo.Verify(r => r.Update(notificacion), Times.Once);
        }

        // Ã¢â€â‚¬Ã¢â€â‚¬ ELIMINAR Ã¢â€â‚¬Ã¢â€â‚¬

        // Caso de Uso: Notificaciones - Proceso: Remover permanentemente una notificaciÃƒÂ³n del historial.
        [Fact]
        public async Task EliminarNotificacion_Existente_LlamarDelete()
        {
            // Arrange
            var notificacion = new Notificacion(Guid.NewGuid(), Guid.NewGuid(), TipoNotificacion.Recordatorio, "M", DateTime.UtcNow);
            _notificacionRepo.Setup(r => r.GetByIdAsync(notificacion.Id)).ReturnsAsync(notificacion);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _useCase.EliminarNotificacionAsync(notificacion.Id);

            // Assert
            _notificacionRepo.Verify(r => r.Delete(notificacion), Times.Once);
        }

        // Ã¢â€â‚¬Ã¢â€â‚¬ ENVIAR NOTIFICACION Ã¢â€â‚¬Ã¢â€â‚¬

        // Caso de Uso: Notificaciones - Proceso: Crear y registrar automÃƒÂ¡ticamente una alerta de vencimiento de prÃƒÂ©stamo.
        [Fact]
        public async Task EnviarNotificacionPrestamo_UsuarioExistente_CreaYGuarda()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var usuario = new Usuario(Guid.NewGuid(), "T", "t@t.com", "h", RolUsuario.Estudiante);
            _usuarioRepo.Setup(r => r.GetByIdAsync(usuarioId)).ReturnsAsync(usuario);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _useCase.EnviarNotificacionPrestamoAsync(usuarioId, DateTime.UtcNow.AddDays(1));

            // Assert
            _notificacionRepo.Verify(r => r.AddAsync(It.IsAny<Notificacion>()), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}
