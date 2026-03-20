using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Infrastructure.Persistance.Configurations
{
    public class PenalizacionConfiguration : IEntityTypeConfiguration<Penalizacion>
    {
        public void Configure(EntityTypeBuilder<Penalizacion> builder)
        {
            builder.ToTable("Penalizaciones");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Motivo)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(p => p.FechaInicio)
                .IsRequired();

            builder.Property(p => p.FechaFin)
                .IsRequired(false);

            builder.Property(p => p.Estado)
                .IsRequired()
                .HasConversion<int>();

            builder.HasOne(p => p.Usuario)
                .WithMany(u => u.Penalizaciones)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Prestamo)
                .WithMany()
                .HasForeignKey(p => p.PrestamoId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}