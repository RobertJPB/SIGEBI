using FluentAssertions;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Enums.Biblioteca;
using SIGEBI.Domain.Enums.Operacion;
using SIGEBI.Domain.Enums.Seguridad;
using SIGEBI.Domain.ValueObjects;
using Xunit;

namespace SIGEBI.Test.Domain
{
    public class PrestamoPolicyTests
    {
        private readonly PrestamoPolicy _policy = new PrestamoPolicy();

        // ── HELPERS ──

        private Usuario CrearUsuarioActivo(RolUsuario rol = RolUsuario.Estudiante)
            => new Usuario(Guid.NewGuid(), "Juan Perez", new Email("juan@test.com"), "hash123", rol);

        private Penalizacion CrearPenalizacionActiva(Guid usuarioId)
            => new Penalizacion(Guid.NewGuid(), usuarioId, "Atraso", 5, DateTime.UtcNow, null);

        // ── PUEDE REALIZAR PRESTAMO ──

        [Fact]
        public void PuedeRealizarPrestamo_UsuarioActivoSinPrestamos_DevuelveTrue()
        {
            // Arrange
            var usuario = CrearUsuarioActivo();

            // Act
            var resultado = _policy.PuedeRealizarPrestamo(usuario, new List<Prestamo>());

            // Assert
            resultado.Should().BeTrue();
        }

        [Fact]
        public void PuedeRealizarPrestamo_EstudianteConLimiteAlcanzado_DevuelveFalse()
        {
            // Arrange
            var usuario = CrearUsuarioActivo(RolUsuario.Estudiante);
            var prestamos = Enumerable.Range(0, 5)
                .Select(_ => new Prestamo(Guid.NewGuid(), usuario.Id, Guid.NewGuid(), 7, DateTime.UtcNow))
                .ToList();

            // Act
            var resultado = _policy.PuedeRealizarPrestamo(usuario, prestamos);

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public void PuedeRealizarPrestamo_DocenteConCuatroPrestamos_DevuelveTrue()
        {
            // Arrange
            var usuario = CrearUsuarioActivo(RolUsuario.Docente);
            var prestamos = Enumerable.Range(0, 4)
                .Select(_ => new Prestamo(Guid.NewGuid(), usuario.Id, Guid.NewGuid(), 7, DateTime.UtcNow))
                .ToList();

            // Act
            var resultado = _policy.PuedeRealizarPrestamo(usuario, prestamos);

            // Assert
            resultado.Should().BeTrue(); // Límite es 5 para docentes
        }

        [Fact]
        public void PuedeRealizarPrestamo_ConPrestamoAtrasado_DevuelveFalse()
        {
            // Arrange
            var usuario = CrearUsuarioActivo(RolUsuario.Docente);
            var prestamoVencido = new Prestamo(Guid.NewGuid(), usuario.Id, Guid.NewGuid(), 15, DateTime.UtcNow.AddDays(-20));
            prestamoVencido.MarcarAtrasadoSiAplica(DateTime.UtcNow);

            // Act
            var resultado = _policy.PuedeRealizarPrestamo(usuario, new List<Prestamo> { prestamoVencido });

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public void PuedeRealizarPrestamo_UsuarioBloqueado_DevuelveFalse()
        {
            // Arrange
            var usuario = CrearUsuarioActivo();
            usuario.Bloquear("Motivo de prueba");

            // Act
            var resultado = _policy.PuedeRealizarPrestamo(usuario, new List<Prestamo>());

            // Assert
            resultado.Should().BeFalse();
        }

        // ── TIENE PENALIZACION ACTIVA ──

        [Fact]
        public void TienePenalizacionActiva_SinPenalizaciones_DevuelveFalse()
        {
            // Act
            var resultado = _policy.TienePenalizacionActiva(new List<Penalizacion>());

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public void TienePenalizacionActiva_ConPenalizacionActiva_DevuelveTrue()
        {
            // Arrange
            var penalizacion = CrearPenalizacionActiva(Guid.NewGuid());

            // Act
            var resultado = _policy.TienePenalizacionActiva(new List<Penalizacion> { penalizacion });

            // Assert
            resultado.Should().BeTrue();
        }

        // ── OBTENER DIAS PLAZO ──

        [Fact]
        public void ObtenerDiasPlazo_Estudiante_DevuelveQuinceDias()
        {
            var dias = _policy.ObtenerDiasPlazo(RolUsuario.Estudiante);
            dias.Should().Be(15);
        }

        [Fact]
        public void ObtenerDiasPlazo_Docente_DevuelveQuinceDias()
        {
            var dias = _policy.ObtenerDiasPlazo(RolUsuario.Docente);
            dias.Should().Be(15);
        }

        [Fact]
        public void ObtenerDiasPlazo_Administrador_DevuelveQuinceDias()
        {
            var dias = _policy.ObtenerDiasPlazo(RolUsuario.Administrador);
            dias.Should().Be(15);
        }
    }
}
