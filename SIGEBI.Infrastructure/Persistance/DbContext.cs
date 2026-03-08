using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Entities.Recursos;

namespace SIGEBI.Infrastructure.Persistance
{
    public class SIGEBIDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public SIGEBIDbContext(DbContextOptions<SIGEBIDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Prestamo> Prestamos { get; set; }

        public DbSet<Categoria> Categorias { get; set; }

        public DbSet<RecursoBibliografico> Recursos { get; set; }

        public DbSet<Valoracion> Valoraciones { get; set; }

        public DbSet<Penalizacion> Penalizaciones { get; set; }

        public DbSet<Notificacion> Notificaciones { get; set; }

        public DbSet<ListaDeseos> ListasDeseos { get; set; }

        public DbSet<Auditoria> Auditorias { get; set; }
    }
}