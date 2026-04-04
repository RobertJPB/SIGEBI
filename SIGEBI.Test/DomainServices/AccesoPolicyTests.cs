using System;
using Xunit;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Domain.Enums.Seguridad;

namespace SIGEBI.Test.DomainServices
{
    public class AccesoPolicyTests
    {
        [Theory]
        [InlineData(RolUsuario.Administrador, true)]
        [InlineData(RolUsuario.Bibliotecario, true)]
        [InlineData(RolUsuario.Estudiante, true)]
        [InlineData(RolUsuario.Docente, true)]
        public void PuedeVerCatalogo_ShouldReturnTrue_ForAllRoles(RolUsuario rol, bool expected)
        {
            // Act
            var result = AccesoPolicy.PuedeVerCatalogo(rol);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(RolUsuario.Administrador, true)]
        [InlineData(RolUsuario.Bibliotecario, true)]
        [InlineData(RolUsuario.Estudiante, false)]
        [InlineData(RolUsuario.Docente, false)]
        public void PuedeGestionarRecursos_ShouldReturnTrue_OnlyForAdminAndLib(RolUsuario rol, bool expected)
        {
            // Act
            var result = AccesoPolicy.PuedeGestionarRecursos(rol);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(RolUsuario.Administrador, true)]
        [InlineData(RolUsuario.Bibliotecario, true)]
        [InlineData(RolUsuario.Estudiante, true)]
        [InlineData(RolUsuario.Docente, true)]
        public void PuedeSolicitarPrestamo_ShouldReturnFalse_ForInvitado(RolUsuario rol, bool expected)
        {
            // Act
            var result = AccesoPolicy.PuedeSolicitarPrestamo(rol);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(RolUsuario.Administrador, true)]
        [InlineData(RolUsuario.Bibliotecario, true)]
        [InlineData(RolUsuario.Estudiante, false)]
        public void PuedeVerTodosLosPrestamos_ShouldBeStaffOnly(RolUsuario rol, bool expected)
        {
            // Act
            var result = AccesoPolicy.PuedeVerTodosLosPrestamos(rol);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ValidarAcceso_ShouldThrowException_WhenConditionIsFalse()
        {
            // Arrange
            var rol = RolUsuario.Estudiante;
            var condicion = false;
            var accion = "borrar base de datos";

            // Act & Assert
            var ex = Assert.Throws<UnauthorizedAccessException>(() => 
                AccesoPolicy.ValidarAcceso(rol, condicion, accion));
            
            Assert.Contains(accion, ex.Message);
            Assert.Contains(rol.ToString(), ex.Message);
        }

        [Fact]
        public void ValidarAcceso_ShouldNotThrow_WhenConditionIsTrue()
        {
            // Arrange
            var rol = RolUsuario.Administrador;
            var condicion = true;

            // Act & Assert (no exception)
            AccesoPolicy.ValidarAcceso(rol, condicion, "alguna accion");
        }
    }
}
