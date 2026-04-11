using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities.Recursos;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Infrastructure.Persistence.Configurations
{
    public class LibroConfiguration : IEntityTypeConfiguration<Libro>
    {
        public void Configure(EntityTypeBuilder<Libro> builder)
        {
            builder.Property(l => l.ISBN)
                .HasColumnName("ISBN")
                .HasMaxLength(20)
                .IsRequired()
                .HasConversion(
                    v => v.Value,
                    v => new ISBN(v));

            builder.Property(l => l.Editorial)
                .HasColumnName("Editorial")
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(l => l.Anio)
                .HasColumnName("Anio")
                .IsRequired();

            builder.Property(l => l.Genero)
                .HasColumnName("Genero")
                .HasMaxLength(50)
                .IsRequired(false);
        }
    }
}
