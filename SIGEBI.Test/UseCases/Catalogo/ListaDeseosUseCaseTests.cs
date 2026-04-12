using FluentAssertions;
using Moq;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.UseCases.Catalogo;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Entities.Recursos;
using SIGEBI.Business.Interfaces.Common;
using SIGEBI.Domain.ValueObjects;
using Xunit;

namespace SIGEBI.Test.UseCases.Catalogo
{
    public class ListaDeseosUseCaseTests
    {
        private readonly Mock<IListaDeseosRepository> _listaRepo;
        private readonly Mock<IRecursoRepository> _recursoRepo;
        private readonly Mock<IUsuarioRepository> _usuarioRepo;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<INotificacionRepository> _notificacionRepo;
        private readonly ListaDeseosUseCase _useCase;

        public ListaDeseosUseCaseTests()
        {
            _listaRepo = new Mock<IListaDeseosRepository>();
            _recursoRepo = new Mock<IRecursoRepository>();
            _usuarioRepo = new Mock<IUsuarioRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _notificacionRepo = new Mock<INotificacionRepository>();

            _useCase = new ListaDeseosUseCase(
                _listaRepo.Object,
                _recursoRepo.Object,
                _usuarioRepo.Object,
                _unitOfWork.Object,
                new Mock<IGuidGenerator>().Object,
                _notificacionRepo.Object);
        }

        // â”€â”€ OBTENER POR USUARIO â”€â”€

        [Fact]
        public async Task ObtenerPorUsuario_ListaExistente_DevuelveDTO()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var lista = new ListaDeseos(Guid.NewGuid(), usuarioId, DateTime.UtcNow);

            _usuarioRepo
                .Setup(r => r.ExistsAsync(usuarioId))
                .ReturnsAsync(true);

            _listaRepo
                .Setup(r => r.GetByUsuarioIdAsync(usuarioId))
                .ReturnsAsync(lista);

            // Act
            var resultado = await _useCase.ObtenerPorUsuarioAsync(usuarioId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.UsuarioId.Should().Be(usuarioId);
        }

        [Fact]
        public async Task ObtenerPorUsuario_ListaNoExiste_CreaYDevuelveDTO()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();

            _usuarioRepo
                .Setup(r => r.ExistsAsync(usuarioId))
                .ReturnsAsync(true);

            _listaRepo
                .Setup(r => r.GetByUsuarioIdAsync(usuarioId))
                .ReturnsAsync((ListaDeseos?)null);

            _listaRepo
                .Setup(r => r.AddAsync(It.IsAny<ListaDeseos>()))
                .Returns(Task.CompletedTask);

            _unitOfWork
                .Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var resultado = await _useCase.ObtenerPorUsuarioAsync(usuarioId);

            // Assert
            resultado.Should().NotBeNull();
            _listaRepo.Verify(r => r.AddAsync(It.IsAny<ListaDeseos>()), Times.Once);
        }

        // â”€â”€ AGREGAR RECURSO â”€â”€

        [Fact]
        public async Task AgregarRecurso_RecursoExistente_AgregaALista()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var libro = new Libro(Guid.NewGuid(), "El Principito", "Antoine", 1, 5, null, new ISBN("9781234567890"), "Editorial X", 1943);
            var lista = new ListaDeseos(Guid.NewGuid(), usuarioId, DateTime.UtcNow);

            _recursoRepo
                .Setup(r => r.GetByIdAsync(libro.Id))
                .ReturnsAsync(libro);

            _usuarioRepo
                .Setup(r => r.ExistsAsync(usuarioId))
                .ReturnsAsync(true);

            _listaRepo
                .Setup(r => r.GetByUsuarioIdAsync(usuarioId))
                .ReturnsAsync(lista);

            _unitOfWork
                .Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            await _useCase.AgregarRecursoAsync(usuarioId, libro.Id);

            // Assert
            lista.Recursos.Should().HaveCount(1);
            lista.Recursos.First().Id.Should().Be(libro.Id);
        }

        [Fact]
        public async Task AgregarRecurso_RecursoNoEncontrado_LanzaExcepcion()
        {
            // Arrange
            _recursoRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((RecursoBibliografico?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _useCase.AgregarRecursoAsync(Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public async Task AgregarRecurso_ListaNoExiste_CreaListaYAgrega()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var libro = new Libro(Guid.NewGuid(), "El Principito", "Antoine", 1, 5, null, new ISBN("9781234567890"), "Editorial X", 1943);

            _recursoRepo
                .Setup(r => r.GetByIdAsync(libro.Id))
                .ReturnsAsync(libro);

            _usuarioRepo
                .Setup(r => r.ExistsAsync(usuarioId))
                .ReturnsAsync(true);

            _listaRepo
                .Setup(r => r.GetByUsuarioIdAsync(usuarioId))
                .ReturnsAsync((ListaDeseos?)null);

            _listaRepo
                .Setup(r => r.AddAsync(It.IsAny<ListaDeseos>()))
                .Returns(Task.CompletedTask);

            _unitOfWork
                .Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act & Assert â€” no lanza excepcion
            await _useCase.Invoking(u => u.AgregarRecursoAsync(usuarioId, libro.Id))
                          .Should().NotThrowAsync();

            _listaRepo.Verify(r => r.AddAsync(It.IsAny<ListaDeseos>()), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Exactly(2));
        }

        // â”€â”€ REMOVER RECURSO â”€â”€

        [Fact]
        public async Task RemoverRecurso_RecursoEnLista_RemueveCorrecto()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var libro = new Libro(Guid.NewGuid(), "El Principito", "Antoine", 1, 5, null, new ISBN("9781234567890"), "Editorial X", 1943);
            var lista = new ListaDeseos(Guid.NewGuid(), usuarioId, DateTime.UtcNow);
            lista.AgregarRecurso(libro);

            _listaRepo
                .Setup(r => r.GetByUsuarioIdAsync(usuarioId))
                .ReturnsAsync(lista);

            _unitOfWork
                .Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            await _useCase.RemoverRecursoAsync(usuarioId, libro.Id);

            // Assert
            lista.Recursos.Should().BeEmpty();
        }

        [Fact]
        public async Task RemoverRecurso_ListaNoEncontrada_LanzaExcepcion()
        {
            // Arrange
            _listaRepo
                .Setup(r => r.GetByUsuarioIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((ListaDeseos?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _useCase.RemoverRecursoAsync(Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public async Task RemoverRecurso_RecursoNoEnLista_LanzaExcepcion()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var lista = new ListaDeseos(Guid.NewGuid(), usuarioId, DateTime.UtcNow);

            _listaRepo
                .Setup(r => r.GetByUsuarioIdAsync(usuarioId))
                .ReturnsAsync(lista);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _useCase.RemoverRecursoAsync(usuarioId, Guid.NewGuid()));
        }
 
        [Fact]
        public async Task ObtenerPorUsuario_UsuarioNoExiste_LanzaExcepcion()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            _usuarioRepo.Setup(r => r.ExistsAsync(usuarioId)).ReturnsAsync(false);
 
            // Act & Assert
            await _useCase.Invoking(u => u.ObtenerPorUsuarioAsync(usuarioId))
                          .Should().ThrowAsync<KeyNotFoundException>();
        }
 
        [Fact]
        public async Task AgregarRecurso_UsuarioNoExiste_LanzaExcepcion()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var recursoId = Guid.NewGuid();
            _recursoRepo.Setup(r => r.GetByIdAsync(recursoId)).ReturnsAsync(new Libro(Guid.NewGuid(), "Test", "Author", 1, 1, null, new ISBN("1234567890"), "Edit", 2020));
            _usuarioRepo.Setup(r => r.ExistsAsync(usuarioId)).ReturnsAsync(false);
 
            // Act & Assert
            await _useCase.Invoking(u => u.AgregarRecursoAsync(usuarioId, recursoId))
                          .Should().ThrowAsync<KeyNotFoundException>();
        }
    }
}
