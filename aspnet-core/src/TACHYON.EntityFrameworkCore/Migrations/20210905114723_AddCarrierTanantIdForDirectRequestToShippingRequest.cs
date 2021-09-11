using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddCarrierTanantIdForDirectRequestToShippingRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CarrierTenantForDirectRequestFKId",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CarrierTenantIdForDirectRequest",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_CarrierTenantForDirectRequestFKId",
                table: "ShippingRequests",
                column: "CarrierTenantForDirectRequestFKId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_AbpTenants_CarrierTenantForDirectRequestFKId",
                table: "ShippingRequests",
                column: "CarrierTenantForDirectRequestFKId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_AbpTenants_CarrierTenantForDirectRequestFKId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_CarrierTenantForDirectRequestFKId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "CarrierTenantForDirectRequestFKId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "CarrierTenantIdForDirectRequest",
                table: "ShippingRequests");
        }
    }
}
