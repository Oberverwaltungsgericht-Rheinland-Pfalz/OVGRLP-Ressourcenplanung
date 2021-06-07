using Microsoft.EntityFrameworkCore.Migrations;

namespace Rema.DbAccess.Migrations
{
    public partial class remindSupporters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Remind",
                table: "SupplierGroups",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SupportersReminded",
                table: "Allocations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remind",
                table: "SupplierGroups");

            migrationBuilder.DropColumn(
                name: "SupportersReminded",
                table: "Allocations");
        }
    }
}
