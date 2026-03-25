using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGEBI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImagenUrlToUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagenUrl",
                table: "Usuarios",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                column: "ImagenUrl",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagenUrl",
                table: "Usuarios");
        }
    }
}
