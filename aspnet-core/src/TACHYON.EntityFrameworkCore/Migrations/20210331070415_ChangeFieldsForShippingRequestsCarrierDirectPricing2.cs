using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class ChangeFieldsForShippingRequestsCarrierDirectPricing2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {





            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestsCarrierDirectPricing_CarrirerTenantId",
                table: "ShippingRequestsCarrierDirectPricing",
                column: "CarrirerTenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestsCarrierDirectPricing_AbpTenants_CarrirerTenantId",
                table: "ShippingRequestsCarrierDirectPricing",
                column: "CarrirerTenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {


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
    }
}
