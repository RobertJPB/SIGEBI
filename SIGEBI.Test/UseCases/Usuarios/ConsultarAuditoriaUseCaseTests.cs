using FluentAssertions;
using Moq;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.UseCases.Usuarios;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Enums.Auditoria;
using Xunit;

namespace SIGEBI.Test.UseCases.Usuarios
{
    public class ConsultarAuditoriaUseCaseTests
    {
        private readonly Mock<IAuditoriaRepository> _auditoriaRepo;
        private readonly ConsultarAuditoriaUseCase _useCase;

        public ConsultarAuditoriaUseCaseTests()
        {
            _auditoriaRepo = new Mock<IAuditoriaRepository>();
            _useCase = new ConsultarAuditoriaUseCase(_auditoriaRepo.Object);
        }

        // ── CONSULTAR AUDITORIA ──

        // Caso de Uso: Consultar Auditoría - Proceso: Obtener el historial completo de acciones del sistema.
        [Fact]
        public async Task ObtenerTodas_HayRegistros_DevuelveLista()
        {
            // Arrange
            var auditorias = new List<Auditoria>
            {
                new Auditoria(Guid.NewGuid(), TipoAccionAuditoria.Crear, "Libros", "Creó un libro", "127.0.0.1", DateTime.UtcNow),
                new Auditoria(Guid.NewGuid(), TipoAccionAuditoria.Actualizar, "Usuarios", "Actualizó perfil", "127.0.0.1", DateTime.UtcNow)
            };
            _auditoriaRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(auditorias);

            // Act
            var resultado = await _useCase.ObtenerTodasAsync();

            // Assert
            resultado.Should().HaveCount(2);
        }

        // Caso de Uso: Consultar Auditoría - Proceso: Filtrar eventos de auditoría por un usuario específico.
        [Fact]
        public async Task ObtenerPorUsuario_UsuarioConAcciones_DevuelveFiltrados()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var auditorias = new List<Auditoria>
            {
                new Auditoria(usuarioId, TipoAccionAuditoria.Crear, "Libros", "Creó un libro", "127.0.0.1", DateTime.UtcNow)
            };
            _auditoriaRepo.Setup(r => r.GetByUsuarioIdAsync(usuarioId)).ReturnsAsync(auditorias);

            // Act
            var resultado = await _useCase.ObtenerPorUsuarioAsync(usuarioId);

            // Assert
            resultado.Should().HaveCount(1);
            resultado.First().UsuarioId.Should().Be(usuarioId);
        }
    }
}
