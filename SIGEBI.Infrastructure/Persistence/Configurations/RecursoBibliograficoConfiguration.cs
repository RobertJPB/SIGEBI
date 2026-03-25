using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Entities.Recursos;

namespace SIGEBI.Infrastructure.Persistence.Configurations
{
    public class RecursoBibliograficoConfiguration : IEntityTypeConfiguration<RecursoBibliografico>
    {
        public void Configure(EntityTypeBuilder<RecursoBibliografico> builder)
        {
            builder.ToTable("RecursosBibliograficos");
            builder.HasKey(r => r.Id);

            // ── GLOBAL QUERY FILTER ──
            // Automáticamente excluye los recursos inactivos (Soft Delete) de TODOS los queries (GetAll, GetById, etc)
            builder.HasQueryFilter(r => r.Estado != SIGEBI.Domain.Enums.Biblioteca.EstadoRecurso.Inactivo);

            builder.HasDiscriminator<string>("TipoRecurso")
                .HasValue<RecursoBibliografico>("Base")
                .HasValue<Libro>("Libro")
                .HasValue<Revista>("Revista")
                .HasValue<Documento>("Documento");

            builder.Property(r => r.Titulo)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(r => r.Autor)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(r => r.Stock)
                .IsRequired();

            builder.Property(r => r.Estado)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(r => r.Descripcion)
                .HasMaxLength(2000)
                .IsRequired(false);

            builder.HasOne(r => r.Categoria)
                .WithMany(c => c.Recursos)
                .HasForeignKey(r => r.IdCategoria)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
