using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGEBI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarGeneroLibro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Genero",
                table: "RecursosBibliograficos",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Genero",
                table: "RecursosBibliograficos");
        }
    }
}
