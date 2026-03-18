using FluentAssertions;
using SIGEBI.Domain.DomainServices;
using Xunit;

namespace SIGEBI.Test.Domain
{
    public class PenalizacionCalculatorTests
    {
        // ── CALCULAR DIAS PENALIZACION ──

        [Fact]
        public void CalcularDiasPenalizacion_SinAtraso_DevuelveCero()
        {
            // Arrange
            var estimada = DateTime.UtcNow;
            var real = DateTime.UtcNow.AddDays(-1); // devuelto antes

            // Act
            var resultado = PenalizacionCalculator.CalcularDiasPenalizacion(estimada, real);

            // Assert
            resultado.Should().Be(0);
        }

        [Fact]
        public void CalcularDiasPenalizacion_UnDiaAtraso_DevuelveTresDias()
        {
            // Arrange
            var estimada = DateTime.UtcNow.AddDays(-1);
            var real = DateTime.UtcNow;

            // Act
            var resultado = PenalizacionCalculator.CalcularDiasPenalizacion(estimada, real);

            // Assert
            resultado.Should().Be(3); // 1 dia atraso * 3 dias penalizacion
        }

        [Fact]
        public void CalcularDiasPenalizacion_MuchosAtrasos_NosuperaMaximo()
        {
            // Arrange
            var estimada = DateTime.UtcNow.AddDays(-100);
            var real = DateTime.UtcNow;

            // Act
            var resultado = PenalizacionCalculator.CalcularDiasPenalizacion(estimada, real);

            // Assert
            resultado.Should().Be(30); // máximo permitido
        }

        [Fact]
        public void CalcularDiasPenalizacion_DevolucionExacta_DevuelveCero()
        {
            // Arrange
            var fecha = DateTime.UtcNow;

            // Act
            var resultado = PenalizacionCalculator.CalcularDiasPenalizacion(fecha, fecha);

            // Assert
            resultado.Should().Be(0);
        }

        // ── TIENE PENALIZACION ──

        [Fact]
        public void TienePenalizacion_DevolucionTardia_DevuelveTrue()
        {
            // Arrange
            var estimada = DateTime.UtcNow.AddDays(-5);
            var real = DateTime.UtcNow;

            // Act
            var resultado = PenalizacionCalculator.TienePenalizacion(estimada, real);

            // Assert
            resultado.Should().BeTrue();
        }

        [Fact]
        public void TienePenalizacion_DevolucionEnTiempo_DevuelveFalse()
        {
            // Arrange
            var estimada = DateTime.UtcNow.AddDays(5);
            var real = DateTime.UtcNow;

            // Act
            var resultado = PenalizacionCalculator.TienePenalizacion(estimada, real);

            // Assert
            resultado.Should().BeFalse();
        }

        // ── OBTENER MOTIVO ──

        [Fact]
        public void ObtenerMotivo_UnDia_DevuelveMensajeCorrecto()
        {
            // Act
            var motivo = PenalizacionCalculator.ObtenerMotivo(1);

            // Assert
            motivo.Should().Contain("1");
            motivo.Should().Contain("atraso");
        }

        [Fact]
        public void ObtenerMotivo_CincoDias_DevuelveMensajeCorrecto()
        {
            // Act
            var motivo = PenalizacionCalculator.ObtenerMotivo(5);

            // Assert
            motivo.Should().Contain("5");
            motivo.Should().Contain("atraso");
        }
    }
}
