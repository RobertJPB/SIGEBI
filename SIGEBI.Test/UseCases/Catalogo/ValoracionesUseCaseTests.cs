using FluentAssertions;
using Moq;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.UseCases.Catalogo;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Entities.Recursos;
using SIGEBI.Domain.Enums.Seguridad;
using Xunit;
using SIGEBI.Business.Interfaces.Common;

namespace SIGEBI.Test.UseCases.Catalogo
{
    public class ValoracionesUseCaseTests
    {
        private readonly Mock<IValoracionRepository> _valoracionRepo;
        private readonly Mock<IUsuarioRepository> _usuarioRepo;
        private readonly Mock<IRecursoRepository> _recursoRepo;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly ValoracionesUseCase _useCase;

        public ValoracionesUseCaseTests()
        {
            _valoracionRepo = new Mock<IValoracionRepository>();
            _usuarioRepo = new Mock<IUsuarioRepository>();
            _recursoRepo = new Mock<IRecursoRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();

            _useCase = new ValoracionesUseCase(
                _valoracionRepo.Object,
                _usuarioRepo.Object,
                _recursoRepo.Object,
                _unitOfWork.Object,
                new Mock<IGuidGenerator>().Object);
        }

        // ── HELPERS ──

        private Usuario CrearUsuario()
            => new Usuario(Guid.NewGuid(), "Juan Perez", "juan@test.com", "hash123", RolUsuario.Estudiante);

        private Libro CrearLibro()
            => new Libro(Guid.NewGuid(), "El Quijote", "Cervantes", 1, 10, null, "123456", "Editorial", 1605);

        // ── AGREGAR VALORACION ──

        // Caso de Uso: Valoraciones - Proceso: Agregar una nueva valoración validando usuario y recurso.
        [Fact]
        public async Task AgregarValoracion_DatosValidos_CreaValoracion()
        {
            // Arrange
            var usuario = CrearUsuario();
            var recurso = CrearLibro();

            _usuarioRepo.Setup(r => r.GetByIdAsync(usuario.Id)).ReturnsAsync(usuario);
            _recursoRepo.Setup(r => r.GetByIdAsync(recurso.Id)).ReturnsAsync(recurso);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var resultado = await _useCase.AgregarValoracionAsync(usuario.Id, recurso.Id, 5, "Excelente");

            // Assert
            resultado.Should().NotBeNull();
            resultado.Calificacion.Should().Be(5);
            _valoracionRepo.Verify(r => r.AddAsync(It.IsAny<Valoracion>()), Times.Once);
        }

        // Caso de Uso: Valoraciones - Proceso: Validar que el usuario existe antes de agregar valoración.
        [Fact]
        public async Task AgregarValoracion_UsuarioNoExiste_LanzaExcepcion()
        {
            // Arrange
            _usuarioRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Usuario?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _useCase.AgregarValoracionAsync(Guid.NewGuid(), Guid.NewGuid(), 5, "Test"));
        }

        // ── OBTENER VALORACIONES ──

        // Caso de Uso: Valoraciones - Proceso: Consultar todas las valoraciones de un recurso específico.
        [Fact]
        public async Task ObtenerValoracionesPorRecurso_HayValoraciones_DevuelveLista()
        {
            // Arrange
            var recursoId = Guid.NewGuid();
            var valoraciones = new List<Valoracion>
            {
                new Valoracion(Guid.NewGuid(), Guid.NewGuid(), recursoId, 4, "Bueno"),
                new Valoracion(Guid.NewGuid(), Guid.NewGuid(), recursoId, 5, "Muy bueno")
            };

            _valoracionRepo.Setup(r => r.GetByRecursoIdAsync(recursoId)).ReturnsAsync(valoraciones);

            // Act
            var resultado = await _useCase.ObtenerValoracionesPorRecursoAsync(recursoId);

            // Assert
            resultado.Should().HaveCount(2);
        }

        // ── PROMEDIO ──

        // Caso de Uso: Valoraciones - Proceso: Calcular el promedio de calificaciones de un recurso.
        [Fact]
        public async Task ObtenerPromedio_CalculaMediaCorrectamente()
        {
            // Arrange
            var recursoId = Guid.NewGuid();
            _valoracionRepo.Setup(r => r.GetPromedioCalificacionAsync(recursoId)).ReturnsAsync(4.5);

            // Act
            var resultado = await _useCase.ObtenerPromedioAsync(recursoId);

            // Assert
            resultado.Should().Be(4.5);
        }

        // ── ELIMINAR ──

        // Caso de Uso: Valoraciones - Proceso: Eliminar una valoración existente por su ID.
        [Fact]
        public async Task EliminarValoracion_Existente_LlamarDeleteYSave()
        {
            // Arrange
            var valoracion = new Valoracion(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 3, "Meh");
            _valoracionRepo.Setup(r => r.GetByIdAsync(valoracion.Id)).ReturnsAsync(valoracion);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _useCase.EliminarValoracionAsync(valoracion.Id);

            // Assert
            _valoracionRepo.Verify(r => r.Delete(valoracion), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}
