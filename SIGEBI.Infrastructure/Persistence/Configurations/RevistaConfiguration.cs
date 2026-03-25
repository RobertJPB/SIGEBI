using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities.Recursos;

namespace SIGEBI.Infrastructure.Persistence.Configurations
{
    public class RevistaConfiguration : IEntityTypeConfiguration<Revista>
    {
        public void Configure(EntityTypeBuilder<Revista> builder)
        {
            builder.Property(r => r.NumeroEdicion)
                .HasColumnName("NumeroEdicion")
                .IsRequired();

            builder.Property(r => r.ISSN)
                .HasColumnName("ISSN")
                .HasMaxLength(20)
                .IsRequired(false);

            builder.Property(r => r.FechaPublicacion)
                .HasColumnName("FechaPublicacion")
                .IsRequired();
        }
    }
}
