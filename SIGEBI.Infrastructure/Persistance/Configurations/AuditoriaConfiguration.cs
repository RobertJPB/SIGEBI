using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Infrastructure.Persistance.Configurations
{
    public class AuditoriaConfiguration : IEntityTypeConfiguration<Auditoria>
    {
        public void Configure(EntityTypeBuilder<Auditoria> builder)
        {
            builder.ToTable("Auditorias");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.TablaAfectada)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Detalle)
                .HasMaxLength(500);

            builder.Property(a => a.IpAddress)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.FechaRegistro)
                .IsRequired();

            builder.Property(a => a.Accion)
                .IsRequired()
                .HasConversion<int>();

            builder.HasOne(a => a.Usuario)
                .WithMany(u => u.Auditorias)
                .HasForeignKey(a => a.UsuarioId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}