using Microsoft.EntityFrameworkCore.Migrations;

namespace Rema.DbAccess.Migrations
{
    public partial class hintsforsuppliers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupplierGroups_Users_UserId",
                table: "SupplierGroups");

            migrationBuilder.DropIndex(
                name: "IX_SupplierGroups_UserId",
                table: "SupplierGroups");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "SupplierGroups");

            migrationBuilder.AddColumn<string>(
                name: "SerializedHints",
                table: "Allocations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SerializedHints",
                table: "Allocations");

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "SupplierGroups",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierGroups_UserId",
                table: "SupplierGroups",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierGroups_Users_UserId",
                table: "SupplierGroups",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
