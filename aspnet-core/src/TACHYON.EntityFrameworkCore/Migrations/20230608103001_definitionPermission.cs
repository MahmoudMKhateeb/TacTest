using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class definitionPermission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportPermissions");

            migrationBuilder.CreateTable(
                name: "ReportDefinitionPermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    EditionId = table.Column<int>(nullable: true),
                    ReportDefinitionId = table.Column<int>(nullable: false),
                    IsGranted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportDefinitionPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportDefinitionPermissions_AbpEditions_EditionId",
                        column: x => x.EditionId,
                        principalTable: "AbpEditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReportDefinitionPermissions_ReportDefinitions_ReportDefinitionId",
                        column: x => x.ReportDefinitionId,
                        principalTable: "ReportDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportDefinitionPermissions_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportDefinitionPermissions_EditionId",
                table: "ReportDefinitionPermissions",
                column: "EditionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportDefinitionPermissions_ReportDefinitionId",
                table: "ReportDefinitionPermissions",
                column: "ReportDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportDefinitionPermissions_TenantId",
                table: "ReportDefinitionPermissions",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportDefinitionPermissions");

            migrationBuilder.CreateTable(
                name: "ReportPermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditionId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsGranted = table.Column<bool>(type: "bit", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    ReportDefinitionId = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportPermissions_AbpEditions_EditionId",
                        column: x => x.EditionId,
                        principalTable: "AbpEditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReportPermissions_ReportDefinitions_ReportDefinitionId",
                        column: x => x.ReportDefinitionId,
                        principalTable: "ReportDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportPermissions_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportPermissions_EditionId",
                table: "ReportPermissions",
                column: "EditionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportPermissions_ReportDefinitionId",
                table: "ReportPermissions",
                column: "ReportDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportPermissions_TenantId",
                table: "ReportPermissions",
                column: "TenantId");
        }
    }
}
