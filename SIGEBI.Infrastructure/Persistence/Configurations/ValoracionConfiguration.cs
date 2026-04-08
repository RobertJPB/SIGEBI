using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Infrastructure.Persistence.Configurations
{
    public class ValoracionConfiguration : IEntityTypeConfiguration<Valoracion>
    {
        public void Configure(EntityTypeBuilder<Valoracion> builder)
        {
            builder.ToTable("Valoraciones");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.Calificacion)
                .IsRequired();

            builder.Property(v => v.Comentario)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.HasOne(v => v.Usuario)
                .WithMany(u => u.Valoraciones)
                .HasForeignKey(v => v.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.Recurso)
                .WithMany()
                .HasForeignKey(v => v.RecursoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Impedir que un usuario valore el mismo recurso mas de una vez
            builder.HasIndex(v => new { v.UsuarioId, v.RecursoId }).IsUnique();
        }
    }
}
