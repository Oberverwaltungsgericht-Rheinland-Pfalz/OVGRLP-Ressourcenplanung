// SPDX-FileCopyrightText: © 2019 Oberverwaltungsgericht Rheinland-Pfalz <poststelle@ovg.jm.rlp.de>
// SPDX-License-Identifier: EUPL-1.2
using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rema.DbAccess.Migrations
{
    public partial class Initial_096 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ressources",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                name: "SupplierGroups",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: false),
                    GroupEmail = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActiveDirectoryID = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Organisation = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gadgets",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true),
                    SuppliedById = table.Column<long>(nullable: true),
                    RessourceId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gadgets", x => x.Id);
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

            migrationBuilder.CreateTable(
                name: "Allocations",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: false),
                    ContactPhone = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(maxLength: 3000, nullable: true),
                    From = table.Column<DateTime>(nullable: false),
                    To = table.Column<DateTime>(nullable: false),
                    IsAllDay = table.Column<bool>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    RessourceId = table.Column<long>(nullable: true),
                    ScheduleSeriesGuid = table.Column<Guid>(nullable: true),
                    CreatedById = table.Column<long>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false),
                    LastModifiedById = table.Column<long>(nullable: true),
                    ApprovedById = table.Column<long>(nullable: true),
                    ApprovedAt = table.Column<DateTime>(nullable: false),
                    ReferencePersonId = table.Column<long>(nullable: true),
                    SerializedHints = table.Column<string>(nullable: true)
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
                name: "IX_Allocations_ReferencePersonId",
                table: "Allocations",
                column: "ReferencePersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Allocations_RessourceId",
                table: "Allocations",
                column: "RessourceId");

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
                name: "IX_SupplierGroups_Title",
                table: "SupplierGroups",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AllocationGagdet");

            migrationBuilder.DropTable(
                name: "Allocations");

            migrationBuilder.DropTable(
                name: "Gadgets");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Ressources");

            migrationBuilder.DropTable(
                name: "SupplierGroups");
        }
    }
}
