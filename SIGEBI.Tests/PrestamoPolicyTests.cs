using SIGEBI.Domain.DomainServices;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Enums.Seguridad;
using Xunit;

namespace SIGEBI.Tests
{
    public class PrestamoPolicyTests
    {
        private Usuario CrearUsuario(RolUsuario rol)
        {
            return new Usuario("Test Usuario", "test@test.com", "hash123", rol);
        }

        [Fact]
        public void Estudiante_TienePlazoDevolucionDe7Dias()
        {
            var usuario = CrearUsuario(RolUsuario.Estudiante);
            var dias = PrestamoPolicy.ObtenerDiasPlazo(usuario);
            Assert.Equal(7, dias);
        }

        [Fact]
        public void Bibliotecario_TienePlazoDevolucionDe14Dias()
        {
            var usuario = CrearUsuario(RolUsuario.Bibliotecario);
            var dias = PrestamoPolicy.ObtenerDiasPlazo(usuario);
            Assert.Equal(14, dias);
        }

        [Fact]
        public void Administrador_TienePlazoDevolucionDe15Dias()
        {
            var usuario = CrearUsuario(RolUsuario.Administrador);
            var dias = PrestamoPolicy.ObtenerDiasPlazo(usuario);
            Assert.Equal(15, dias);
        }

        [Fact]
        public void UsuarioInactivo_NoPuedeRealizarPrestamo()
        {
            var usuario = CrearUsuario(RolUsuario.Estudiante);
            usuario.Desactivar();
            var puede = PrestamoPolicy.PuedeRealizarPrestamo(usuario, new List<Prestamo>());
            Assert.False(puede);
        }

        [Fact]
        public void UsuarioSinPenalizaciones_NoPoseeBloqueo()
        {
            var puede = PrestamoPolicy.TienePenalizacionActiva(new List<Penalizacion>());
            Assert.False(puede);
        }
    }
}