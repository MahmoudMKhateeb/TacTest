using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class add_foreignKeys_report_definition_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ReportPermissions_EditionId",
                table: "ReportPermissions",
                column: "EditionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportPermissions_TenantId",
                table: "ReportPermissions",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportPermissions_AbpEditions_EditionId",
                table: "ReportPermissions",
                column: "EditionId",
                principalTable: "AbpEditions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportPermissions_AbpTenants_TenantId",
                table: "ReportPermissions",
                column: "TenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportPermissions_AbpEditions_EditionId",
                table: "ReportPermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportPermissions_AbpTenants_TenantId",
                table: "ReportPermissions");

            migrationBuilder.DropIndex(
                name: "IX_ReportPermissions_EditionId",
                table: "ReportPermissions");

            migrationBuilder.DropIndex(
                name: "IX_ReportPermissions_TenantId",
                table: "ReportPermissions");
        }
    }
}
