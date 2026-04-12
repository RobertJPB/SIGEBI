using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGEBI.Infrastructure.Migrations
{
    public partial class AddPrestamoIdToPenalizacion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Valoraciones_UsuarioId",
                table: "Valoraciones");

            migrationBuilder.AddColumn<Guid>(
                name: "PrestamoId",
                table: "Penalizaciones",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UsuarioId",
                table: "Auditorias",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Valoraciones_UsuarioId_RecursoId",
                table: "Valoraciones",
                columns: new[] { "UsuarioId", "RecursoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Penalizaciones_PrestamoId",
                table: "Penalizaciones",
                column: "PrestamoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Penalizaciones_Prestamos_PrestamoId",
                table: "Penalizaciones",
                column: "PrestamoId",
                principalTable: "Prestamos",
                principalColumn: "Id");
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Penalizaciones_Prestamos_PrestamoId",
                table: "Penalizaciones");

            migrationBuilder.DropIndex(
                name: "IX_Valoraciones_UsuarioId_RecursoId",
                table: "Valoraciones");

            migrationBuilder.DropIndex(
                name: "IX_Penalizaciones_PrestamoId",
                table: "Penalizaciones");

            migrationBuilder.DropColumn(
                name: "PrestamoId",
                table: "Penalizaciones");

            migrationBuilder.AlterColumn<Guid>(
                name: "UsuarioId",
                table: "Auditorias",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Valoraciones_UsuarioId",
                table: "Valoraciones",
                column: "UsuarioId");
        }
    }
}

