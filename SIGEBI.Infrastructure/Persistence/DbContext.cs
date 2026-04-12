using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Entities.Recursos;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Infrastructure.Persistence
{
    public class SIGEBIDbContext : DbContext
    {
        public SIGEBIDbContext(DbContextOptions<SIGEBIDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; } 
        public DbSet<RecursoBibliografico> RecursosBibliograficos { get; set; }
        public DbSet<Libro> Libros { get; set; } 
        public DbSet<Revista> Revistas { get; set; } 
        public DbSet<Documento> Documentos { get; set; }
        public DbSet<Prestamo> Prestamos { get; set; } 
        public DbSet<Categoria> Categorias { get; set; } 
        public DbSet<Penalizacion> Penalizaciones { get; set; } 
        public DbSet<Valoracion> Valoraciones { get; set; } 
        public DbSet<Notificacion> Notificaciones { get; set; } 
        public DbSet<ListaDeseos> ListasDeseos { get; set; } 
        public DbSet<Auditoria> Auditorias { get; set; }
        public DbSet<Reporte> Reportes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SIGEBIDbContext).Assembly);

            // Seed del Usuario Administrador para Pruebas Iniciales
            modelBuilder.Entity<Usuario>().HasData(new
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Nombre = "Administrador",
                Correo = (Email)"admin@sigebi.com",
                ContrasenaHash = "JAvlGPu9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=", // admin123
                Rol = Domain.Enums.Seguridad.RolUsuario.Bibliotecario,
                Estado = Domain.Enums.Seguridad.EstadoUsuario.Activo,
                FechaRegistro = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });

            // Seed del Usuario del Sistema para Auditoría
            modelBuilder.Entity<Usuario>().HasData(new
            {
                Id = UsuarioIdSistema,
                Nombre = "Sistema",
                Correo = (Email)"sistema@sigebi.com",
                ContrasenaHash = Usuario.SistemaHashCentinela,
                Rol = Domain.Enums.Seguridad.RolUsuario.Bibliotecario,
                Estado = Domain.Enums.Seguridad.EstadoUsuario.Activo,
                FechaRegistro = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });

            // Exclusión explícita del flujo de consultas (para evitar login y visibilidad)
            modelBuilder.Entity<Usuario>().HasQueryFilter(u => u.ContrasenaHash != Usuario.SistemaHashCentinela);
        }

        private static readonly Guid UsuarioIdSistema = Guid.Parse("00000000-0000-0000-0000-000000000001");
    }
}
