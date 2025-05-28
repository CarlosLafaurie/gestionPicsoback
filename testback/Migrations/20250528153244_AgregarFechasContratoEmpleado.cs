using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace testback.Migrations
{
    /// <inheritdoc />
    public partial class AgregarFechasContratoEmpleado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ✅ Ya no agregamos la columna "Ubicacion"

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaFinContrato",
                table: "Empleado",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaInicioContrato",
                table: "Empleado",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "NumeroCuenta",
                table: "Empleado",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "Empleado",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ❌ Tampoco eliminamos "Ubicacion"

            migrationBuilder.DropColumn(
                name: "FechaFinContrato",
                table: "Empleado");

            migrationBuilder.DropColumn(
                name: "FechaInicioContrato",
                table: "Empleado");

            migrationBuilder.DropColumn(
                name: "NumeroCuenta",
                table: "Empleado");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "Empleado");
        }
    }
}
