using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Infrastructure.Persistance.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuarios");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Correo)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasIndex(u => u.Correo)
                .IsUnique();

            builder.Property(u => u.ContrasenaHash)
                .IsRequired();

            builder.Property(u => u.Rol)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(u => u.Estado)
                .IsRequired()
                .HasConversion<int>();

            builder.HasMany(u => u.Prestamos)
                .WithOne(p => p.Usuario)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Penalizaciones)
                .WithOne(p => p.Usuario)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Notificaciones)
                .WithOne(n => n.Usuario)
                .HasForeignKey(n => n.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Valoraciones)
                .WithOne(v => v.Usuario)
                .HasForeignKey(v => v.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Auditorias)
     .WithOne(a => a.Usuario)
     .HasForeignKey(a => a.UsuarioId)
     .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.ListaDeseos)
                .WithOne(l => l.Usuario)
                .HasForeignKey<ListaDeseos>(l => l.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}