using Microsoft.EntityFrameworkCore.Migrations;

namespace Rema.DbAccess.Migrations
{
    public partial class deactivatableRessources : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeactivated",
                table: "Ressources",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeactivated",
                table: "Gadgets",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeactivated",
                table: "Ressources");

            migrationBuilder.DropColumn(
                name: "IsDeactivated",
                table: "Gadgets");
        }
    }
}
