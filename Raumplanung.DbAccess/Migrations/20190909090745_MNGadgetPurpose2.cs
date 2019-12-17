using Microsoft.EntityFrameworkCore.Migrations;

namespace DbRaumplanung.Migrations
{
    public partial class MNGadgetPurpose2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GadgetPurpose_AllocationPurposes_AllocationPurposeId",
                table: "GadgetPurpose");

            migrationBuilder.DropForeignKey(
                name: "FK_GadgetPurpose_Gadgets_GadgetId",
                table: "GadgetPurpose");

            migrationBuilder.DropForeignKey(
                name: "FK_Gadgets_AllocationPurposes_AllocationPurposeId",
                table: "Gadgets");

            migrationBuilder.DropIndex(
                name: "IX_Gadgets_AllocationPurposeId",
                table: "Gadgets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GadgetPurpose",
                table: "GadgetPurpose");

            migrationBuilder.DropColumn(
                name: "AllocationPurposeId",
                table: "Gadgets");

            migrationBuilder.RenameTable(
                name: "GadgetPurpose",
                newName: "GadgetPurposes");

            migrationBuilder.RenameIndex(
                name: "IX_GadgetPurpose_AllocationPurposeId",
                table: "GadgetPurposes",
                newName: "IX_GadgetPurposes_AllocationPurposeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GadgetPurposes",
                table: "GadgetPurposes",
                columns: new[] { "GadgetId", "AllocationPurposeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_GadgetPurposes_AllocationPurposes_AllocationPurposeId",
                table: "GadgetPurposes",
                column: "AllocationPurposeId",
                principalTable: "AllocationPurposes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GadgetPurposes_Gadgets_GadgetId",
                table: "GadgetPurposes",
                column: "GadgetId",
                principalTable: "Gadgets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GadgetPurposes_AllocationPurposes_AllocationPurposeId",
                table: "GadgetPurposes");

            migrationBuilder.DropForeignKey(
                name: "FK_GadgetPurposes_Gadgets_GadgetId",
                table: "GadgetPurposes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GadgetPurposes",
                table: "GadgetPurposes");

            migrationBuilder.RenameTable(
                name: "GadgetPurposes",
                newName: "GadgetPurpose");

            migrationBuilder.RenameIndex(
                name: "IX_GadgetPurposes_AllocationPurposeId",
                table: "GadgetPurpose",
                newName: "IX_GadgetPurpose_AllocationPurposeId");

            migrationBuilder.AddColumn<long>(
                name: "AllocationPurposeId",
                table: "Gadgets",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GadgetPurpose",
                table: "GadgetPurpose",
                columns: new[] { "GadgetId", "AllocationPurposeId" });

            migrationBuilder.CreateIndex(
                name: "IX_Gadgets_AllocationPurposeId",
                table: "Gadgets",
                column: "AllocationPurposeId");

            migrationBuilder.AddForeignKey(
                name: "FK_GadgetPurpose_AllocationPurposes_AllocationPurposeId",
                table: "GadgetPurpose",
                column: "AllocationPurposeId",
                principalTable: "AllocationPurposes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GadgetPurpose_Gadgets_GadgetId",
                table: "GadgetPurpose",
                column: "GadgetId",
                principalTable: "Gadgets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Gadgets_AllocationPurposes_AllocationPurposeId",
                table: "Gadgets",
                column: "AllocationPurposeId",
                principalTable: "AllocationPurposes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
