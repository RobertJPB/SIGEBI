using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGEBI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarImagenUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagenUrl",
                table: "RecursosBibliograficos",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagenUrl",
                table: "RecursosBibliograficos");
        }
    }
}
