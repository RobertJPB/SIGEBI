using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Infrastructure.Persistance.Configurations
{
    public class NotificacionConfiguration : IEntityTypeConfiguration<Notificacion>
    {
        public void Configure(EntityTypeBuilder<Notificacion> builder)
        {
            builder.ToTable("Notificaciones");

            builder.HasKey(n => n.Id);

            builder.Property(n => n.Mensaje)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(n => n.Fecha)
                .IsRequired();

            builder.Property(n => n.Tipo)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(n => n.Estado)
                .IsRequired()
                .HasConversion<int>();

            builder.HasOne(n => n.Usuario)
                .WithMany(u => u.Notificaciones)
                .HasForeignKey(n => n.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}