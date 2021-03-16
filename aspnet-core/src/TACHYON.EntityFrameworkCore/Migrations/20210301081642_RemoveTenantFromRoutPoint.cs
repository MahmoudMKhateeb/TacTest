using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class RemoveTenantFromRoutPoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoutPoints_AbpTenants_TenantId",
                table: "RoutPoints");

            migrationBuilder.DropIndex(
                name: "IX_RoutPoints_TenantId",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "RoutPoints");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "RoutPoints",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RoutPoints_TenantId",
                table: "RoutPoints",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoutPoints_AbpTenants_TenantId",
                table: "RoutPoints",
                column: "TenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
