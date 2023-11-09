// SPDX-FileCopyrightText: © 2019 Oberverwaltungsgericht Rheinland-Pfalz <poststelle@ovg.jm.rlp.de>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rema.DbAccess.Migrations
{
    public partial class cleanUpDB2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gadgets_Ressources_RessourceId",
                table: "Gadgets");

            migrationBuilder.DropTable(
                name: "AllocationGagdetOld");

            migrationBuilder.DropIndex(
                name: "IX_Gadgets_RessourceId",
                table: "Gadgets");

            migrationBuilder.DropColumn(
                name: "Usability",
                table: "Ressources");

            migrationBuilder.DropColumn(
                name: "RessourceId",
                table: "Gadgets");

            migrationBuilder.DropColumn(
                name: "RessourceId",
                table: "Allocations");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Usability",
                table: "Ressources",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RessourceId",
                table: "Gadgets",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RessourceId",
                table: "Allocations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "AllocationGagdetOld",
                columns: table => new
                {
                    AllocationId = table.Column<long>(type: "bigint", nullable: false),
                    GadgetId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllocationGagdetOld", x => new { x.AllocationId, x.GadgetId });
                    table.ForeignKey(
                        name: "FK_AllocationGagdetOld_Allocations_AllocationId",
                        column: x => x.AllocationId,
                        principalTable: "Allocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AllocationGagdetOld_Gadgets_GadgetId",
                        column: x => x.GadgetId,
                        principalTable: "Gadgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Gadgets_RessourceId",
                table: "Gadgets",
                column: "RessourceId");

            migrationBuilder.CreateIndex(
                name: "IX_AllocationGagdetOld_GadgetId",
                table: "AllocationGagdetOld",
                column: "GadgetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Gadgets_Ressources_RessourceId",
                table: "Gadgets",
                column: "RessourceId",
                principalTable: "Ressources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
