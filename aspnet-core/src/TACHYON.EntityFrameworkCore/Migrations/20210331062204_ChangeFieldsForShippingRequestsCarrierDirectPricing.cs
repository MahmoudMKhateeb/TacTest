using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class ChangeFieldsForShippingRequestsCarrierDirectPricing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CarrirTenantId",
                table: "ShippingRequestsCarrierDirectPricing",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CarrirerTenantId",
                table: "ShippingRequestsCarrierDirectPricing",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestsCarrierDirectPricing_CarrirTenantId",
                table: "ShippingRequestsCarrierDirectPricing",
                column: "CarrirTenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestsCarrierDirectPricing_AbpTenants_CarrirTenantId",
                table: "ShippingRequestsCarrierDirectPricing",
                column: "CarrirTenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestsCarrierDirectPricing_AbpTenants_CarrirTenantId",
                table: "ShippingRequestsCarrierDirectPricing");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestsCarrierDirectPricing_CarrirTenantId",
                table: "ShippingRequestsCarrierDirectPricing");

            migrationBuilder.DropColumn(
                name: "CarrirTenantId",
                table: "ShippingRequestsCarrierDirectPricing");

            migrationBuilder.DropColumn(
                name: "CarrirerTenantId",
                table: "ShippingRequestsCarrierDirectPricing");
        }
    }
}
