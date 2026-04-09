using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Infrastructure.Persistence.Configurations
{
    public class SolicitudAccesoConfiguration : IEntityTypeConfiguration<SolicitudAcceso>
    {
        public void Configure(EntityTypeBuilder<SolicitudAcceso> builder)
        {
            builder.ToTable("SolicitudesAcceso");
            builder.HasKey(s => s.Id);

            builder.Property(s => s.FechaSolicitud)
                .IsRequired();

            builder.Property(s => s.FueAprobada)
                .IsRequired();

            builder.Property(s => s.MotivoRechazo)
                .HasMaxLength(500)
                .IsRequired(false);

            // Relaciones
            builder.HasOne(s => s.Usuario)
                .WithMany() // No navegamos desde Usuario a Solicitudes en el dominio actual
                .HasForeignKey(s => s.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Recurso)
                .WithMany() // No navegamos desde Recurso a Solicitudes en el dominio actual
                .HasForeignKey(s => s.RecursoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
