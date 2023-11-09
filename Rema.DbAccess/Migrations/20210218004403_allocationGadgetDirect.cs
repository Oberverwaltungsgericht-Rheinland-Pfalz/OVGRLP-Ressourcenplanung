// SPDX-FileCopyrightText: © 2019 Oberverwaltungsgericht Rheinland-Pfalz <poststelle@ovg.jm.rlp.de>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rema.DbAccess.Migrations
{
    public partial class allocationGadgetDirect : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AllocationGagdet_Allocations_AllocationId",
                table: "AllocationGagdet");

            migrationBuilder.DropForeignKey(
                name: "FK_AllocationGagdet_Gadgets_GadgetId",
                table: "AllocationGagdet");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AllocationGagdet",
                table: "AllocationGagdet");

            migrationBuilder.RenameTable(
                name: "AllocationGagdet",
                newName: "AllocationGagdetOld");

            migrationBuilder.RenameIndex(
                name: "IX_AllocationGagdet_GadgetId",
                table: "AllocationGagdetOld",
                newName: "IX_AllocationGagdetOld_GadgetId");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Gadgets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AllocationGagdetOld",
                table: "AllocationGagdetOld",
                columns: new[] { "AllocationId", "GadgetId" });

            migrationBuilder.CreateTable(
                name: "AllocationGadget",
                columns: table => new
                {
                    AllocationsId = table.Column<long>(type: "bigint", nullable: false),
                    GadgetsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllocationGadget", x => new { x.AllocationsId, x.GadgetsId });
                    table.ForeignKey(
                        name: "FK_AllocationGadget_Allocations_AllocationsId",
                        column: x => x.AllocationsId,
                        principalTable: "Allocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AllocationGadget_Gadgets_GadgetsId",
                        column: x => x.GadgetsId,
                        principalTable: "Gadgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AllocationGadget_GadgetsId",
                table: "AllocationGadget",
                column: "GadgetsId");

            migrationBuilder.AddForeignKey(
                name: "FK_AllocationGagdetOld_Allocations_AllocationId",
                table: "AllocationGagdetOld",
                column: "AllocationId",
                principalTable: "Allocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AllocationGagdetOld_Gadgets_GadgetId",
                table: "AllocationGagdetOld",
                column: "GadgetId",
                principalTable: "Gadgets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AllocationGagdetOld_Allocations_AllocationId",
                table: "AllocationGagdetOld");

            migrationBuilder.DropForeignKey(
                name: "FK_AllocationGagdetOld_Gadgets_GadgetId",
                table: "AllocationGagdetOld");

            migrationBuilder.DropTable(
                name: "AllocationGadget");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AllocationGagdetOld",
                table: "AllocationGagdetOld");

            migrationBuilder.RenameTable(
                name: "AllocationGagdetOld",
                newName: "AllocationGagdet");

            migrationBuilder.RenameIndex(
                name: "IX_AllocationGagdetOld_GadgetId",
                table: "AllocationGagdet",
                newName: "IX_AllocationGagdet_GadgetId");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Gadgets",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AllocationGagdet",
                table: "AllocationGagdet",
                columns: new[] { "AllocationId", "GadgetId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AllocationGagdet_Allocations_AllocationId",
                table: "AllocationGagdet",
                column: "AllocationId",
                principalTable: "Allocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AllocationGagdet_Gadgets_GadgetId",
                table: "AllocationGagdet",
                column: "GadgetId",
                principalTable: "Gadgets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
