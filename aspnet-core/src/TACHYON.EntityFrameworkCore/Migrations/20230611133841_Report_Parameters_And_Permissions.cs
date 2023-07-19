using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Report_Parameters_And_Permissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "ReportDefinitionPermissions");

            migrationBuilder.DropColumn(
                name: "CreatorUserId",
                table: "ReportDefinitionPermissions");

            migrationBuilder.DropColumn(
                name: "DeleterUserId",
                table: "ReportDefinitionPermissions");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "ReportDefinitionPermissions");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "ReportDefinitionPermissions");

            migrationBuilder.DropColumn(
                name: "LastModifierUserId",
                table: "ReportDefinitionPermissions");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Reports",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Format",
                table: "Reports",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ReportPermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RoleId = table.Column<int>(nullable: true),
                    UserId = table.Column<long>(nullable: true),
                    ReportId = table.Column<Guid>(nullable: false),
                    IsGranted = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportPermissions_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportPermissions_AbpRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AbpRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReportPermissions_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reports_TenantId",
                table: "Reports",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportPermissions_ReportId",
                table: "ReportPermissions",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportPermissions_RoleId",
                table: "ReportPermissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportPermissions_UserId",
                table: "ReportPermissions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AbpTenants_TenantId",
                table: "Reports",
                column: "TenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AbpTenants_TenantId",
                table: "Reports");

            migrationBuilder.DropTable(
                name: "ReportPermissions");

            migrationBuilder.DropIndex(
                name: "IX_Reports_TenantId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "Format",
                table: "Reports");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "ReportDefinitionPermissions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatorUserId",
                table: "ReportDefinitionPermissions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DeleterUserId",
                table: "ReportDefinitionPermissions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "ReportDefinitionPermissions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "ReportDefinitionPermissions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LastModifierUserId",
                table: "ReportDefinitionPermissions",
                type: "bigint",
                nullable: true);
        }
    }
}
