using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace testback.Migrations
{
    /// <inheritdoc />
    public partial class RemoveColumnsFromEmpleados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comentarios",
                table: "Empleado");

            migrationBuilder.DropColumn(
                name: "fecha_hora_entrada",
                table: "Empleado");

            migrationBuilder.DropColumn(
                name: "fecha_hora_salida",
                table: "Empleado");

            migrationBuilder.DropColumn(
                name: "permisos_especiales",
                table: "Empleado");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comentarios",
                table: "Empleado",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "fecha_hora_entrada",
                table: "Empleado",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "fecha_hora_salida",
                table: "Empleado",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "permisos_especiales",
                table: "Empleado",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
