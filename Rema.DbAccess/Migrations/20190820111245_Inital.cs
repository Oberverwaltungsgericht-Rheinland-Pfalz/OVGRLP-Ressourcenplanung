using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rema.DbAccess.Migrations
{
  public partial class Inital : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "AllocationPurposes",
          columns: table => new
          {
            Id = table.Column<long>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            Title = table.Column<string>(nullable: false),
            Description = table.Column<string>(maxLength: 3000, nullable: true),
            Notes = table.Column<string>(maxLength: 3000, nullable: true),
            ContactPhone = table.Column<string>(nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_AllocationPurposes", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "Ressources",
          columns: table => new
          {
            Id = table.Column<long>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            Name = table.Column<string>(nullable: false),
            FunctionDescription = table.Column<string>(nullable: true),
            Usability = table.Column<string>(nullable: true),
            SpecialsDescription = table.Column<string>(nullable: true),
            Type = table.Column<string>(nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Ressources", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "Users",
          columns: table => new
          {
            Id = table.Column<long>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            Name = table.Column<string>(nullable: false),
            Email = table.Column<string>(nullable: false),
            Mobile = table.Column<string>(nullable: true),
            Phone = table.Column<string>(nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Users", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "Allocations",
          columns: table => new
          {
            Id = table.Column<long>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            From = table.Column<DateTime>(nullable: false),
            To = table.Column<DateTime>(nullable: false),
            IsAllDay = table.Column<bool>(nullable: false),
            Status = table.Column<int>(nullable: false),
            RessourceId = table.Column<long>(nullable: true),
            PurposeId = table.Column<long>(nullable: true),
            CreatedById = table.Column<long>(nullable: false),
            CreatedAt = table.Column<DateTime>(nullable: false),
            LastModified = table.Column<DateTime>(nullable: false),
            LastModifiedById = table.Column<long>(nullable: true),
            ApprovedById = table.Column<long>(nullable: true),
            ApprovedAt = table.Column<DateTime>(nullable: false),
            ReferencePersonId = table.Column<long>(nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Allocations", x => x.Id);
            table.ForeignKey(
                      name: "FK_Allocations_Users_ApprovedById",
                      column: x => x.ApprovedById,
                      principalTable: "Users",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Restrict);
            table.ForeignKey(
                      name: "FK_Allocations_Users_CreatedById",
                      column: x => x.CreatedById,
                      principalTable: "Users",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_Allocations_Users_LastModifiedById",
                      column: x => x.LastModifiedById,
                      principalTable: "Users",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Restrict);
            table.ForeignKey(
                      name: "FK_Allocations_AllocationPurposes_PurposeId",
                      column: x => x.PurposeId,
                      principalTable: "AllocationPurposes",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Restrict);
            table.ForeignKey(
                      name: "FK_Allocations_Users_ReferencePersonId",
                      column: x => x.ReferencePersonId,
                      principalTable: "Users",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Restrict);
            table.ForeignKey(
                      name: "FK_Allocations_Ressources_RessourceId",
                      column: x => x.RessourceId,
                      principalTable: "Ressources",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateTable(
          name: "SupplierGroups",
          columns: table => new
          {
            Id = table.Column<long>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            Title = table.Column<string>(nullable: false),
            GroupEmail = table.Column<string>(nullable: true),
            UserId = table.Column<long>(nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_SupplierGroups", x => x.Id);
            table.ForeignKey(
                      name: "FK_SupplierGroups_Users_UserId",
                      column: x => x.UserId,
                      principalTable: "Users",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateTable(
          name: "Gadgets",
          columns: table => new
          {
            Id = table.Column<long>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            Title = table.Column<string>(nullable: true),
            SuppliedById = table.Column<long>(nullable: true),
            AllocationPurposeId = table.Column<long>(nullable: true),
            RessourceId = table.Column<long>(nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Gadgets", x => x.Id);
            table.ForeignKey(
                      name: "FK_Gadgets_AllocationPurposes_AllocationPurposeId",
                      column: x => x.AllocationPurposeId,
                      principalTable: "AllocationPurposes",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Restrict);
            table.ForeignKey(
                      name: "FK_Gadgets_Ressources_RessourceId",
                      column: x => x.RessourceId,
                      principalTable: "Ressources",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Restrict);
            table.ForeignKey(
                      name: "FK_Gadgets_SupplierGroups_SuppliedById",
                      column: x => x.SuppliedById,
                      principalTable: "SupplierGroups",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateIndex(
          name: "IX_Allocations_ApprovedById",
          table: "Allocations",
          column: "ApprovedById");

      migrationBuilder.CreateIndex(
          name: "IX_Allocations_CreatedById",
          table: "Allocations",
          column: "CreatedById");

      migrationBuilder.CreateIndex(
          name: "IX_Allocations_LastModifiedById",
          table: "Allocations",
          column: "LastModifiedById");

      migrationBuilder.CreateIndex(
          name: "IX_Allocations_PurposeId",
          table: "Allocations",
          column: "PurposeId");

      migrationBuilder.CreateIndex(
          name: "IX_Allocations_ReferencePersonId",
          table: "Allocations",
          column: "ReferencePersonId");

      migrationBuilder.CreateIndex(
          name: "IX_Allocations_RessourceId",
          table: "Allocations",
          column: "RessourceId");

      migrationBuilder.CreateIndex(
          name: "IX_Gadgets_AllocationPurposeId",
          table: "Gadgets",
          column: "AllocationPurposeId");

      migrationBuilder.CreateIndex(
          name: "IX_Gadgets_RessourceId",
          table: "Gadgets",
          column: "RessourceId");

      migrationBuilder.CreateIndex(
          name: "IX_Gadgets_SuppliedById",
          table: "Gadgets",
          column: "SuppliedById");

      migrationBuilder.CreateIndex(
          name: "IX_Ressources_Name",
          table: "Ressources",
          column: "Name");

      migrationBuilder.CreateIndex(
          name: "IX_SupplierGroups_UserId",
          table: "SupplierGroups",
          column: "UserId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "Allocations");

      migrationBuilder.DropTable(
          name: "Gadgets");

      migrationBuilder.DropTable(
          name: "AllocationPurposes");

      migrationBuilder.DropTable(
          name: "Ressources");

      migrationBuilder.DropTable(
          name: "SupplierGroups");

      migrationBuilder.DropTable(
          name: "Users");
    }
  }
}
