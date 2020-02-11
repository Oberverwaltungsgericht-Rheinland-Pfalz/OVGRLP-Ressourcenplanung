using Microsoft.EntityFrameworkCore.Migrations;

namespace Rema.DbAccess.Migrations
{
    public partial class AllocationReplacePurpose : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gadgets_Allocations_AllocationId",
                table: "Gadgets");

            migrationBuilder.DropIndex(
                name: "IX_Gadgets_AllocationId",
                table: "Gadgets");

            migrationBuilder.DropColumn(
                name: "AllocationId",
                table: "Gadgets");

            migrationBuilder.CreateTable(
                name: "AllocationGagdet",
                columns: table => new
                {
                    AllocationId = table.Column<long>(nullable: false),
                    GadgetId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllocationGagdet", x => new { x.AllocationId, x.GadgetId });
                    table.ForeignKey(
                        name: "FK_AllocationGagdet_Allocations_AllocationId",
                        column: x => x.AllocationId,
                        principalTable: "Allocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AllocationGagdet_Gadgets_GadgetId",
                        column: x => x.GadgetId,
                        principalTable: "Gadgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AllocationGagdet_GadgetId",
                table: "AllocationGagdet",
                column: "GadgetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AllocationGagdet");

            migrationBuilder.AddColumn<long>(
                name: "AllocationId",
                table: "Gadgets",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Gadgets_AllocationId",
                table: "Gadgets",
                column: "AllocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Gadgets_Allocations_AllocationId",
                table: "Gadgets",
                column: "AllocationId",
                principalTable: "Allocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
