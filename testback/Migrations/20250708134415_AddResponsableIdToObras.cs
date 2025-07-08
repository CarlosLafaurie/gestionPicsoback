using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace testback.Migrations
{
    /// <inheritdoc />
    public partial class AddResponsableIdToObras : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_solicitud_Inventario_inventarioId",
                table: "solicitud");

            migrationBuilder.DropPrimaryKey(
                name: "PK_solicitud",
                table: "solicitud");

            migrationBuilder.DropIndex(
                name: "IX_solicitud_inventarioId",
                table: "solicitud");

            migrationBuilder.DropColumn(
                name: "Obra",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "cantidad",
                table: "solicitud");

            migrationBuilder.DropColumn(
                name: "inventarioId",
                table: "solicitud");

            migrationBuilder.DropColumn(
                name: "Responsable",
                table: "Obra");

            migrationBuilder.RenameTable(
                name: "solicitud",
                newName: "Solicitud");

            migrationBuilder.RenameColumn(
                name: "RutaDocumento",
                table: "documentopermiso",
                newName: "NombreArchivo");

            migrationBuilder.AddColumn<int>(
                name: "ObraId",
                table: "Usuario",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ResponsableId",
                table: "Obra",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Cantidad",
                table: "Movimiento",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCompra",
                table: "Inventario",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Garantia",
                table: "Inventario",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Marca",
                table: "Inventario",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Proveedor",
                table: "Inventario",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "Archivo",
                table: "documentopermiso",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Solicitud",
                table: "Solicitud",
                column: "id");

            migrationBuilder.CreateTable(
                name: "InventarioInterno",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventarioId = table.Column<int>(type: "int", nullable: false),
                    Obra = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResponsableObra = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Usando = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CantidadAsignada = table.Column<int>(type: "int", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioInterno", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventarioInterno_Inventario_InventarioId",
                        column: x => x.InventarioId,
                        principalTable: "Inventario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RevisionInventario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventarioId = table.Column<int>(type: "int", nullable: false),
                    FechaRevision = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Responsable = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Encontrado = table.Column<bool>(type: "bit", nullable: false),
                    EstadoFisico = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RevisionInventario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RevisionInventario_Inventario_InventarioId",
                        column: x => x.InventarioId,
                        principalTable: "Inventario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "solicitud_item",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    solicitudId = table.Column<int>(type: "int", nullable: false),
                    inventarioId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_solicitud_item", x => x.id);
                    table.ForeignKey(
                        name: "FK_solicitud_item_Inventario_inventarioId",
                        column: x => x.inventarioId,
                        principalTable: "Inventario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_solicitud_item_Solicitud_solicitudId",
                        column: x => x.solicitudId,
                        principalTable: "Solicitud",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_ObraId",
                table: "Usuario",
                column: "ObraId",
                unique: true,
                filter: "[ObraId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioInterno_InventarioId",
                table: "InventarioInterno",
                column: "InventarioId");

            migrationBuilder.CreateIndex(
                name: "IX_RevisionInventario_InventarioId",
                table: "RevisionInventario",
                column: "InventarioId");

            migrationBuilder.CreateIndex(
                name: "IX_solicitud_item_inventarioId",
                table: "solicitud_item",
                column: "inventarioId");

            migrationBuilder.CreateIndex(
                name: "IX_solicitud_item_solicitudId",
                table: "solicitud_item",
                column: "solicitudId");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuario_Obra_ObraId",
                table: "Usuario",
                column: "ObraId",
                principalTable: "Obra",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuario_Obra_ObraId",
                table: "Usuario");

            migrationBuilder.DropTable(
                name: "InventarioInterno");

            migrationBuilder.DropTable(
                name: "RevisionInventario");

            migrationBuilder.DropTable(
                name: "solicitud_item");

            migrationBuilder.DropIndex(
                name: "IX_Usuario_ObraId",
                table: "Usuario");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Solicitud",
                table: "Solicitud");

            migrationBuilder.DropColumn(
                name: "ObraId",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "ResponsableId",
                table: "Obra");

            migrationBuilder.DropColumn(
                name: "Cantidad",
                table: "Movimiento");

            migrationBuilder.DropColumn(
                name: "FechaCompra",
                table: "Inventario");

            migrationBuilder.DropColumn(
                name: "Garantia",
                table: "Inventario");

            migrationBuilder.DropColumn(
                name: "Marca",
                table: "Inventario");

            migrationBuilder.DropColumn(
                name: "Proveedor",
                table: "Inventario");

            migrationBuilder.DropColumn(
                name: "Archivo",
                table: "documentopermiso");

            migrationBuilder.RenameTable(
                name: "Solicitud",
                newName: "solicitud");

            migrationBuilder.RenameColumn(
                name: "NombreArchivo",
                table: "documentopermiso",
                newName: "RutaDocumento");

            migrationBuilder.AddColumn<string>(
                name: "Obra",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "cantidad",
                table: "solicitud",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "inventarioId",
                table: "solicitud",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Responsable",
                table: "Obra",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_solicitud",
                table: "solicitud",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_solicitud_inventarioId",
                table: "solicitud",
                column: "inventarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_solicitud_Inventario_inventarioId",
                table: "solicitud",
                column: "inventarioId",
                principalTable: "Inventario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
