using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Entities.Recursos;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
           
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SIGEBIDbContext).Assembly);

            // Seed del Usuario del Sistema para Auditoría
            modelBuilder.Entity<Usuario>().HasData(new
            {
                Id = UsuarioIdSistema,
                Nombre = "Sistema",
                Correo = "sistema@sigebi.com",
                ContrasenaHash = "SYSTEM_ACCOUNT_NO_LOGIN",
                Rol = Domain.Enums.Seguridad.RolUsuario.Bibliotecario,
                Estado = Domain.Enums.Seguridad.EstadoUsuario.Activo
            });
        }

        private static readonly Guid UsuarioIdSistema = Guid.Parse("00000000-0000-0000-0000-000000000001");
    }
}
