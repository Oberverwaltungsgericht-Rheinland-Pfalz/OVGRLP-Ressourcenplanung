using Microsoft.EntityFrameworkCore.Migrations;

namespace Rema.DbAccess.Migrations
{
    public partial class _20Multiroom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Allocations_Ressources_RessourceId",
                table: "Allocations");

            migrationBuilder.DropForeignKey(
                name: "FK_Allocations_Users_CreatedById",
                table: "Allocations");

            migrationBuilder.DropIndex(
                name: "IX_Allocations_RessourceId",
                table: "Allocations");

            migrationBuilder.AlterColumn<long>(
                name: "RessourceId",
                table: "Allocations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CreatedById",
                table: "Allocations",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateTable(
                name: "AllocationRessource",
                columns: table => new
                {
                    AllocationsId = table.Column<long>(type: "bigint", nullable: false),
                    RessourcesId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllocationRessource", x => new { x.AllocationsId, x.RessourcesId });
                    table.ForeignKey(
                        name: "FK_AllocationRessource_Allocations_AllocationsId",
                        column: x => x.AllocationsId,
                        principalTable: "Allocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AllocationRessource_Ressources_RessourcesId",
                        column: x => x.RessourcesId,
                        principalTable: "Ressources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AllocationRessource_RessourcesId",
                table: "AllocationRessource",
                column: "RessourcesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Allocations_Users_CreatedById",
                table: "Allocations",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Allocations_Users_CreatedById",
                table: "Allocations");

            migrationBuilder.DropTable(
                name: "AllocationRessource");

            migrationBuilder.AlterColumn<long>(
                name: "RessourceId",
                table: "Allocations",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "CreatedById",
                table: "Allocations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Allocations_RessourceId",
                table: "Allocations",
                column: "RessourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Allocations_Ressources_RessourceId",
                table: "Allocations",
                column: "RessourceId",
                principalTable: "Ressources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Allocations_Users_CreatedById",
                table: "Allocations",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
