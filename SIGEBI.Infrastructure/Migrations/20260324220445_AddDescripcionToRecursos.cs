using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGEBI.Infrastructure.Migrations
{
    public partial class AddDescripcionToRecursos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Penalizaciones_Prestamos_PrestamoId",
                table: "Penalizaciones");

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "RecursosBibliograficos",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Penalizaciones_Prestamos_PrestamoId",
                table: "Penalizaciones",
                column: "PrestamoId",
                principalTable: "Prestamos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Penalizaciones_Prestamos_PrestamoId",
                table: "Penalizaciones");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "RecursosBibliograficos");

            migrationBuilder.AddForeignKey(
                name: "FK_Penalizaciones_Prestamos_PrestamoId",
                table: "Penalizaciones",
                column: "PrestamoId",
                principalTable: "Prestamos",
                principalColumn: "Id");
        }
    }
}

