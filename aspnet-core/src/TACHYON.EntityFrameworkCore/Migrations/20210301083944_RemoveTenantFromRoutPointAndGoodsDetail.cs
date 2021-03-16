using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class RemoveTenantFromRoutPointAndGoodsDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoutPoints_AbpTenants_TenantId",
                table: "RoutPoints");

            migrationBuilder.DropIndex(
                name: "IX_RoutPoints_TenantId",
                table: "RoutPoints");

            migrationBuilder.DropIndex(
                name: "IX_GoodsDetails_TenantId",
                table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "GoodsDetails");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "RoutPoints",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "GoodsDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RoutPoints_TenantId",
                table: "RoutPoints",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsDetails_TenantId",
                table: "GoodsDetails",
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
