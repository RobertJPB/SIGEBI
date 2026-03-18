using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Entities.Recursos;

namespace SIGEBI.Infrastructure.Persistance
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
            // Si agregamos una nueva tabla, no modificamos este DbContext gigante.
            // Simplemente creamos su archivo de configuracion (IEntityTypeConfiguration) 
            // y esta linea lo levanta automaticamente (abierto a extension, cerrado a modificacion).
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SIGEBIDbContext).Assembly);
        }

        // ID de sistema para la auditoria cuando no hay un usuario logueado (como en procesos automaticos)
        private static readonly Guid UsuarioIdSistema = Guid.Parse("00000000-0000-0000-0000-000000000001");

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // 1. Detectamos todas las entidades que alguien haya insertado, modificado o borrado
            var entradasModificadas = ChangeTracker.Entries()
                .Where(e => (e.State == EntityState.Added || 
                             e.State == EntityState.Modified || 
                             e.State == EntityState.Deleted) && 
                             e.Entity is not Auditoria) // Evitamos auditar la propia tabla de auditoria
                .ToList();

            foreach (var entrada in entradasModificadas)
            {
                // 2. Determinamos qué tipo de acción fue (Crear, Editar, Borrar)
                var accion = entrada.State switch
                {
                    EntityState.Added => Domain.Enums.Auditoria.TipoAccionAuditoria.Crear,
                    EntityState.Modified => Domain.Enums.Auditoria.TipoAccionAuditoria.Actualizar,
                    EntityState.Deleted => Domain.Enums.Auditoria.TipoAccionAuditoria.Eliminar,
                    _ => Domain.Enums.Auditoria.TipoAccionAuditoria.Actualizar
                };

                var nombreTabla = entrada.Entity.GetType().Name;

                // 3. Creamos el registro de auditoría automáticamente
                var auditoriaAutomatica = new Auditoria(
                    usuarioId: UsuarioIdSistema, // TODO: Inyectar IHttpContextAccessor en el futuro
                    accion: accion,
                    tablaAfectada: nombreTabla,
                    detalle: $"Cambio automático detectado en {nombreTabla}. Estado EF: {entrada.State}",
                    ipAddress: "::1", // Localhost
                    fechaRegistroUtc: DateTime.UtcNow
                );

                Auditorias.Add(auditoriaAutomatica);
            }

            // 4. Dejamos que Entity Framework guarde todo (los datos reales + nuestra auditoría)
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
