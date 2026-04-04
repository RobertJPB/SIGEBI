using FluentAssertions;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Enums.Biblioteca;
using SIGEBI.Domain.Enums.Operacion;
using SIGEBI.Domain.Enums.Seguridad;
using Xunit;

namespace SIGEBI.Test.Domain
{
    public class PremiumPolicyTests
    {
        // â”€â”€ HELPERS â”€â”€

        private Usuario CrearUsuarioActivo(RolUsuario rol = RolUsuario.Estudiante)
            => new Usuario(Guid.NewGuid(), "Juan Perez", "juan@test.com", "hash123", rol);

        private Penalizacion CrearPenalizacionActiva(Guid usuarioId)
            => new Penalizacion(Guid.NewGuid(), usuarioId, "Atraso", 5, DateTime.UtcNow, null);

        // â”€â”€ PUEDE REALIZAR PRESTAMO â”€â”€

        [Fact]
        public void PuedeRealizarPrestamo_UsuarioActivoSinPrestamos_DevuelveTrue()
        {
            // Arrange
            var usuario = CrearUsuarioActivo();

            // Act
            var resultado = PrestamoPolicy.PuedeRealizarPrestamo(usuario, new List<Prestamo>());

            // Assert
            resultado.Should().BeTrue();
        }

        [Fact]
        public void PuedeRealizarPrestamo_EstudianteConLimiteAlcanzado_DevuelveFalse()
        {
            // Arrange
            var usuario = CrearUsuarioActivo(RolUsuario.Estudiante);
            var prestamos = Enumerable.Range(0, 3)
                .Select(_ => new Prestamo(Guid.NewGuid(), usuario.Id, Guid.NewGuid(), 7, DateTime.UtcNow))
                .ToList();

            // Act
            var resultado = PrestamoPolicy.PuedeRealizarPrestamo(usuario, prestamos);

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public void PuedeRealizarPrestamo_UsuarioBloqueado_DevuelveFalse()
        {
            // Arrange
            var usuario = CrearUsuarioActivo();
            usuario.Bloquear();

            // Act
            var resultado = PrestamoPolicy.PuedeRealizarPrestamo(usuario, new List<Prestamo>());

            // Assert
            resultado.Should().BeFalse();
        }

        // â”€â”€ TIENE PENALIZACION ACTIVA â”€â”€

        [Fact]
        public void TienePenalizacionActiva_SinPenalizaciones_DevuelveFalse()
        {
            // Act
            var resultado = PrestamoPolicy.TienePenalizacionActiva(new List<Penalizacion>());

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public void TienePenalizacionActiva_ConPenalizacionActiva_DevuelveTrue()
        {
            // Arrange
            var penalizacion = CrearPenalizacionActiva(Guid.NewGuid());

            // Act
            var resultado = PrestamoPolicy.TienePenalizacionActiva(new List<Penalizacion> { penalizacion });

            // Assert
            resultado.Should().BeTrue();
        }

        // â”€â”€ OBTENER DIAS PLAZO â”€â”€

        [Fact]
        public void ObtenerDiasPlazo_Estudiante_DevuelteQuinceDias()
        {
            var dias = PrestamoPolicy.ObtenerDiasPlazo();
            dias.Should().Be(15);
        }

        [Fact]
        public void ObtenerDiasPlazo_Administrador_DevuelveQuinceDias()
        {
            var dias = PrestamoPolicy.ObtenerDiasPlazo();
            dias.Should().Be(15);
        }

        [Fact]
        public void ObtenerDiasPlazo_Bibliotecario_DevuelteQuinceDias()
        {
            var dias = PrestamoPolicy.ObtenerDiasPlazo();
            dias.Should().Be(15);
        }
    }
}
