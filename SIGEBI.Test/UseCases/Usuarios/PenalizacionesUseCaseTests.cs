using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.UseCases.Usuarios;
using SIGEBI.Domain.Entities;
using SIGEBI.Business.Interfaces.Common;
using Xunit;

namespace SIGEBI.Test.UseCases.Usuarios
{
    public class PenalizacionesUseCaseTests
    {
        private readonly Mock<IPrestamoRepository> _prestamoRepo;
        private readonly Mock<IUsuarioRepository> _usuarioRepo;
        private readonly Mock<IPenalizacionRepository> _penalizacionRepo;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly PenalizacionesUseCase _useCase;

        public PenalizacionesUseCaseTests()
        {
            _prestamoRepo = new Mock<IPrestamoRepository>();
            _usuarioRepo = new Mock<IUsuarioRepository>();
            _penalizacionRepo = new Mock<IPenalizacionRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();

            _useCase = new PenalizacionesUseCase(
                _prestamoRepo.Object,
                _usuarioRepo.Object,
                _penalizacionRepo.Object,
                _unitOfWork.Object,
                new Mock<IGuidGenerator>().Object,
                Microsoft.Extensions.Logging.Abstractions.NullLogger<PenalizacionesUseCase>.Instance);
        }

        // â”€â”€ APLICAR PENALIZACIONES AUTOMATICAS â”€â”€

        // Caso de Uso: Penalizaciones - Proceso: Identificar prÃ©stamos atrasados y generar sanciones automÃ¡ticamente.
        [Fact]
        public async Task AplicarPenalizaciones_HayAtrasados_CreaPenalizaciones()
        {
            // Arrange
            var prestamo = new Prestamo(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 7, DateTime.UtcNow.AddDays(-10));
            // Forzamos que sea atrasado para el test
            var prestamosAtrasados = new List<Prestamo> { prestamo };
            _prestamoRepo.Setup(r => r.GetAtrasadosAsync()).ReturnsAsync(prestamosAtrasados);
            // Sin penalizaciÃ³n previa para el prÃ©stamo â†’ el use case debe crear una
            _penalizacionRepo.Setup(r => r.GetByPrestamoIdAsync(prestamo.Id)).ReturnsAsync((Penalizacion?)null);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _useCase.AplicarPenalizacionesAsync();

            // Assert
            _penalizacionRepo.Verify(r => r.AddAsync(It.IsAny<Penalizacion>()), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        // â”€â”€ APLICAR PENALIZACION MANUAL â”€â”€

        // Caso de Uso: Penalizaciones - Proceso: Registrar una sanciÃ³n de forma manual por parte de un administrador.
        [Fact]
        public async Task AplicarPenalizacionManual_DatosValidos_CreaYGuarda()
        {
            // Arrange
            var dto = new AplicarPenalizacionManualDTO
            {
                UsuarioId = Guid.NewGuid(),
                Motivo = "Mal comportamiento",
                DiasPenalizacion = 5
            };
            var usuario = new SIGEBI.Domain.Entities.Usuario(Guid.NewGuid(), "T", "t@t.com", "h", SIGEBI.Domain.Enums.Seguridad.RolUsuario.Estudiante);
            _usuarioRepo.Setup(r => r.GetByIdAsync(dto.UsuarioId)).ReturnsAsync(usuario);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _useCase.AplicarPenalizacionManualAsync(dto);

            // Assert
            _penalizacionRepo.Verify(r => r.AddAsync(It.IsAny<Penalizacion>()), Times.Once);
        }

        // â”€â”€ OBTENER PENALIZACIONES â”€â”€

        // Caso de Uso: Penalizaciones - Proceso: Consultar el historial completo de sanciones aplicadas.
        [Fact]
        public async Task ObtenerTodas_LlamaAlRepo()
        {
            // Arrange
            _penalizacionRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Penalizacion>());

            // Act
            await _useCase.ObtenerTodasLasPenalizacionesAsync();

            // Assert
            _penalizacionRepo.Verify(r => r.GetAllAsync(), Times.Once);
        }

        // â”€â”€ FINALIZAR PENALIZACION â”€â”€

        // Caso de Uso: Penalizaciones - Proceso: Dar por terminada una sanciÃ³n antes de su fecha de expiraciÃ³n original.
        [Fact]
        public async Task FinalizarPenalizacion_Existente_CambiaEstadoAFinalizada()
        {
            // Arrange
            var penalizacion = new Penalizacion(Guid.NewGuid(), Guid.NewGuid(), "M", 3, DateTime.UtcNow);
            _penalizacionRepo.Setup(r => r.GetByIdAsync(penalizacion.Id)).ReturnsAsync(penalizacion);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _useCase.FinalizarPenalizacionAsync(penalizacion.Id);

            // Assert
            penalizacion.Estado.Should().Be(SIGEBI.Domain.Enums.Operacion.EstadoPenalizacion.Finalizada);
            _penalizacionRepo.Verify(r => r.Update(penalizacion), Times.Once);
        }

        // â”€â”€ ELIMINAR PENALIZACION â”€â”€

        // Caso de Uso: Penalizaciones - Proceso: Eliminar permanentemente una sanciÃ³n para corregir errores administrativos.
        [Fact]
        public async Task EliminarPenalizacion_Existente_LlamarDelete()
        {
            // Arrange
            var penalizacion = new Penalizacion(Guid.NewGuid(), Guid.NewGuid(), "M", 3, DateTime.UtcNow);
            _penalizacionRepo.Setup(r => r.GetByIdAsync(penalizacion.Id)).ReturnsAsync(penalizacion);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _useCase.EliminarPenalizacionAsync(penalizacion.Id);

            // Assert
            _penalizacionRepo.Verify(r => r.Delete(penalizacion), Times.Once);
        }
    }
}
