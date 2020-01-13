using Microsoft.EntityFrameworkCore.Migrations;

namespace DbRaumplanung.Migrations
{
  public partial class adUser2 : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AlterColumn<string>(
          name: "Email",
          table: "Users",
          nullable: false,
          oldClrType: typeof(string));

      migrationBuilder.CreateIndex(
          name: "IX_Users_Email",
          table: "Users",
          column: "Email");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropIndex(
          name: "IX_Users_Email",
          table: "Users");

      migrationBuilder.AlterColumn<string>(
          name: "Email",
          table: "Users",
          nullable: false,
          oldClrType: typeof(string));
    }
  }
}
