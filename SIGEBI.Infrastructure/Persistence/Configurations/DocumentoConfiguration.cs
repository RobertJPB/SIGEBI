using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities.Recursos;

namespace SIGEBI.Infrastructure.Persistence.Configurations
{
    public class DocumentoConfiguration : IEntityTypeConfiguration<Documento>
    {
        public void Configure(EntityTypeBuilder<Documento> builder)
        {
            builder.Property(d => d.Formato)
                .HasColumnName("Formato")
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property(d => d.Institucion)
                .HasColumnName("Institucion")
                .HasMaxLength(150)
                .IsRequired(false);

            builder.Property(d => d.Anio)
                .HasColumnName("Anio")
                .IsRequired();
        }
    }
}
