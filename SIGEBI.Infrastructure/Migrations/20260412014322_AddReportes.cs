using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGEBI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReportes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolicitudesAcceso");

            /* Parche de sincronización: Estas columnas ya existen en la base de datos real */
            /*
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegistro",
                table: "Usuarios",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "RecursosBibliograficos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "NumeroPaginas",
                table: "RecursosBibliograficos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioCreadorId",
                table: "RecursosBibliograficos",
                type: "uniqueidentifier",
                nullable: true);
            */

            migrationBuilder.CreateTable(
                name: "Reportes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    FechaGeneracion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Parametros = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Resultado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GeneradoPorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reportes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reportes_Usuarios_GeneradoPorId",
                        column: x => x.GeneradoPorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            /*
            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                column: "FechaRegistro",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "ContrasenaHash", "Correo", "Estado", "FechaRegistro", "ImagenUrl", "MotivoEstado", "Nombre", "Rol" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000002"), "JAvlGPu9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=", "admin@sigebi.com", 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Administrador", 2 });
            */

            migrationBuilder.CreateIndex(
                name: "IX_RecursosBibliograficos_UsuarioCreadorId",
                table: "RecursosBibliograficos",
                column: "UsuarioCreadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Reportes_GeneradoPorId",
                table: "Reportes",
                column: "GeneradoPorId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecursosBibliograficos_Usuarios_UsuarioCreadorId",
                table: "RecursosBibliograficos",
                column: "UsuarioCreadorId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecursosBibliograficos_Usuarios_UsuarioCreadorId",
                table: "RecursosBibliograficos");

            migrationBuilder.DropTable(
                name: "Reportes");

            migrationBuilder.DropIndex(
                name: "IX_RecursosBibliograficos_UsuarioCreadorId",
                table: "RecursosBibliograficos");

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"));

            /*
            migrationBuilder.DropColumn(
                name: "FechaRegistro",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "RecursosBibliograficos");

            migrationBuilder.DropColumn(
                name: "NumeroPaginas",
                table: "RecursosBibliograficos");

            migrationBuilder.DropColumn(
                name: "UsuarioCreadorId",
                table: "RecursosBibliograficos");
            */

            migrationBuilder.CreateTable(
                name: "SolicitudesAcceso",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecursoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FueAprobada = table.Column<bool>(type: "bit", nullable: false),
                    MotivoRechazo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesAcceso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudesAcceso_RecursosBibliograficos_RecursoId",
                        column: x => x.RecursoId,
                        principalTable: "RecursosBibliograficos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudesAcceso_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesAcceso_RecursoId",
                table: "SolicitudesAcceso",
                column: "RecursoId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesAcceso_UsuarioId",
                table: "SolicitudesAcceso",
                column: "UsuarioId");
        }
    }
}
