using Microsoft.EntityFrameworkCore.Migrations;

namespace DbRaumplanung.Migrations
{
    public partial class MNGadgetPurpose : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "SupplierGroups",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.CreateTable(
                name: "GadgetPurpose",
                columns: table => new
                {
                    GadgetId = table.Column<long>(nullable: false),
                    AllocationPurposeId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GadgetPurpose", x => new { x.GadgetId, x.AllocationPurposeId });
                    table.ForeignKey(
                        name: "FK_GadgetPurpose_AllocationPurposes_AllocationPurposeId",
                        column: x => x.AllocationPurposeId,
                        principalTable: "AllocationPurposes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GadgetPurpose_Gadgets_GadgetId",
                        column: x => x.GadgetId,
                        principalTable: "Gadgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierGroups_Title",
                table: "SupplierGroups",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_GadgetPurpose_AllocationPurposeId",
                table: "GadgetPurpose",
                column: "AllocationPurposeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GadgetPurpose");

            migrationBuilder.DropIndex(
                name: "IX_SupplierGroups_Title",
                table: "SupplierGroups");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "SupplierGroups",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
