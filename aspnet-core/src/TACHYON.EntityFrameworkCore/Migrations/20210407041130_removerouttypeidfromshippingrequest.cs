using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class removerouttypeidfromshippingrequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_Routes_RouteId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_RouteId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_Routes_TenantId",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "RouteId",
                table: "ShippingRequests");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RouteId",
                table: "ShippingRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_RouteId",
                table: "ShippingRequests",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_TenantId",
                table: "Routes",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_Routes_RouteId",
                table: "ShippingRequests",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
