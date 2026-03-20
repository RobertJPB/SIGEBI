using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Entities.Recursos;

namespace SIGEBI.Infrastructure.Persistance.Configurations
{
    public class ListaDeseosConfiguration : IEntityTypeConfiguration<ListaDeseos>
    {
        public void Configure(EntityTypeBuilder<ListaDeseos> builder)
        {
            builder.ToTable("ListasDeseos");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.FechaCreacion)
                .IsRequired();

            builder.HasOne(l => l.Usuario)
                .WithOne(u => u.ListaDeseos)
                .HasForeignKey<ListaDeseos>(l => l.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(l => l.Recursos)
                .WithMany()
                .UsingEntity(j => j.ToTable("ListaDeseosRecursos"));

            // Forzamos el uso del backing field para evitar problemas de tracking
            builder.Navigation(l => l.Recursos)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}