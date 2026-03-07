using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Infrastructure.Persistance.Configurations
{
    public class PrestamoConfiguration : IEntityTypeConfiguration<Prestamo>
    {
        public void Configure(EntityTypeBuilder<Prestamo> builder)
        {
            builder.ToTable("Prestamos");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.FechaInicio)
                .IsRequired();

            builder.Property(p => p.FechaDevolucionEstimada)
                .IsRequired();

            builder.Property(p => p.FechaDevolucionReal)
                .IsRequired(false);

            builder.Property(p => p.EstadoActual)
                .IsRequired()
                .HasConversion<int>();

            builder.HasOne(p => p.Usuario)
                .WithMany(u => u.Prestamos)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Recurso)
                .WithMany()
                .HasForeignKey(p => p.RecursoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}