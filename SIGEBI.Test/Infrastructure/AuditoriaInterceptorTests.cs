using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.ValueObjects;
using SIGEBI.Infrastructure.Persistence;
using SIGEBI.Infrastructure.Persistence.Interceptors;
using Xunit;

namespace SIGEBI.Test.Infrastructure
{
    public class AuditoriaInterceptorTests
    {
        private Mock<IHttpContextAccessor> CreateMockHttpContext(string ip, string? userId = null)
        {
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse(ip);
            
            if (userId != null)
            {
                var claims = new System.Collections.Generic.List<System.Security.Claims.Claim>
                {
                    new System.Security.Claims.Claim("sub", userId)
                };
                var identity = new System.Security.Claims.ClaimsIdentity(claims, "TestAuth");
                context.User = new System.Security.Claims.ClaimsPrincipal(identity);
            }

            mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);
            return mockHttpContextAccessor;
        }

        [Fact]
        public async Task SaveChanges_ShouldCaptureRealClientIP()
        {
            // Arrange
            string testIp = "192.168.1.50";
            var mockAccessor = CreateMockHttpContext(testIp);
            var interceptor = new AuditoriaInterceptor(mockAccessor.Object);

            var options = new DbContextOptionsBuilder<SIGEBIDbContext>()
                .UseSqlite("DataSource=:memory:")
                .AddInterceptors(interceptor)
                .Options;

            using var context = new SIGEBIDbContext(options);
            await context.Database.OpenConnectionAsync();
            await context.Database.EnsureCreatedAsync();

            // Arrange: Crear un usuario para evitar error de FK
            var userId = Guid.NewGuid();
            var user = new Usuario(userId, "Test", new Email("test@mail.com"), "pass", SIGEBI.Domain.Enums.Seguridad.RolUsuario.Estudiante);
            context.Usuarios.Add(user);
            await context.SaveChangesAsync();

            // Act: Crear una categoría
            var categoria = new Categoria("Test Categoria");
            context.Categorias.Add(categoria);
            await context.SaveChangesAsync();

            // Assert
            var auditoria = await context.Auditorias.OrderByDescending(a => a.FechaRegistro).FirstOrDefaultAsync();
            Assert.NotNull(auditoria);
            Assert.Equal(testIp, auditoria.IpAddress);
            Assert.Equal(SIGEBI.Domain.Enums.Auditoria.TipoAccionAuditoria.Crear, auditoria.Accion);
        }

        [Fact]
        public async Task SaveChanges_ShouldCaptureUserIdFromClaims()
        {
            // Arrange
            string testUserId = Guid.NewGuid().ToString();
            var mockAccessor = CreateMockHttpContext("127.0.0.1", testUserId);
            var interceptor = new AuditoriaInterceptor(mockAccessor.Object);

            var options = new DbContextOptionsBuilder<SIGEBIDbContext>()
                .UseSqlite("DataSource=:memory:")
                .AddInterceptors(interceptor)
                .Options;

            using var context = new SIGEBIDbContext(options);
            await context.Database.OpenConnectionAsync();
            await context.Database.EnsureCreatedAsync();

            // Arrange: Crear el usuario que viene en el claim
            var userId = Guid.Parse(testUserId);
            var user = new Usuario(userId, "Test", new Email("test@mail.com"), "pass", SIGEBI.Domain.Enums.Seguridad.RolUsuario.Estudiante);
            context.Usuarios.Add(user);
            await context.SaveChangesAsync();

            // Act: Crear una categoría
            var categoria = new Categoria("Test Categoria 2");
            context.Categorias.Add(categoria);
            await context.SaveChangesAsync();

            // Assert
            var auditoria = await context.Auditorias.OrderByDescending(a => a.FechaRegistro).FirstOrDefaultAsync();
            Assert.NotNull(auditoria);
            Assert.Equal(userId, auditoria.UsuarioId);
        }
    }
}
