using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addCarrierTenantIdtoShippingRequeststable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CarrierTenantId",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_CarrierTenantId",
                table: "ShippingRequests",
                column: "CarrierTenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_AbpTenants_CarrierTenantId",
                table: "ShippingRequests",
                column: "CarrierTenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_AbpTenants_CarrierTenantId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_CarrierTenantId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "CarrierTenantId",
                table: "ShippingRequests");
        }
    }
}
