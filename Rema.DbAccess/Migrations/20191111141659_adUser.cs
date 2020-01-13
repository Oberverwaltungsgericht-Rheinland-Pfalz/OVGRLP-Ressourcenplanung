using Microsoft.EntityFrameworkCore.Migrations;

namespace Rema.DbAccess.Migrations
{
  public partial class adUser : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.RenameColumn(
          name: "Phone",
          table: "Users",
          newName: "Organisation");

      migrationBuilder.RenameColumn(
          name: "Mobile",
          table: "Users",
          newName: "ActiveDirectoryID");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.RenameColumn(
          name: "Organisation",
          table: "Users",
          newName: "Phone");

      migrationBuilder.RenameColumn(
          name: "ActiveDirectoryID",
          table: "Users",
          newName: "Mobile");
    }
  }
}
