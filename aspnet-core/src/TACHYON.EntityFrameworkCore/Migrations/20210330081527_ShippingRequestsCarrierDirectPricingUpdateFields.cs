using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class ShippingRequestsCarrierDirectPricingUpdateFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinalPrice",
                table: "ShippingRequestsCarrierDirectPricing");

            migrationBuilder.DropColumn(
                name: "MinValueComission",
                table: "ShippingRequestsCarrierDirectPricing");

            migrationBuilder.DropColumn(
                name: "PercentageComission",
                table: "ShippingRequestsCarrierDirectPricing");

            migrationBuilder.DropColumn(
                name: "ValueComission",
                table: "ShippingRequestsCarrierDirectPricing");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountSubTotal",
                table: "ShippingRequestsCarrierDirectPricing",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionValue",
                table: "ShippingRequestsCarrierDirectPricing",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinCommissionValue",
                table: "ShippingRequestsCarrierDirectPricing",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentCommission",
                table: "ShippingRequestsCarrierDirectPricing",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SettingsCommissionValue",
                table: "ShippingRequestsCarrierDirectPricing",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SettingsMinCommissionValue",
                table: "ShippingRequestsCarrierDirectPricing",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SettingsPercentCommission",
                table: "ShippingRequestsCarrierDirectPricing",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "ShippingRequestsCarrierDirectPricing",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCommission",
                table: "ShippingRequestsCarrierDirectPricing",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VatAmount",
                table: "ShippingRequestsCarrierDirectPricing",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestsCarrierDirectPricing_TenantId",
                table: "ShippingRequestsCarrierDirectPricing",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestsCarrierDirectPricing_AbpTenants_TenantId",
                table: "ShippingRequestsCarrierDirectPricing",
                column: "TenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestsCarrierDirectPricing_AbpTenants_TenantId",
                table: "ShippingRequestsCarrierDirectPricing");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestsCarrierDirectPricing_TenantId",
                table: "ShippingRequestsCarrierDirectPricing");

            migrationBuilder.DropColumn(
                name: "AmountSubTotal",
                table: "ShippingRequestsCarrierDirectPricing");

            migrationBuilder.DropColumn(
                name: "CommissionValue",
                table: "ShippingRequestsCarrierDirectPricing");

            migrationBuilder.DropColumn(
                name: "MinCommissionValue",
                table: "ShippingRequestsCarrierDirectPricing");

            migrationBuilder.DropColumn(
                name: "PercentCommission",
                table: "ShippingRequestsCarrierDirectPricing");

            migrationBuilder.DropColumn(
                name: "SettingsCommissionValue",
                table: "ShippingRequestsCarrierDirectPricing");

            migrationBuilder.DropColumn(
                name: "SettingsMinCommissionValue",
                table: "ShippingRequestsCarrierDirectPricing");

            migrationBuilder.DropColumn(
                name: "SettingsPercentCommission",
                table: "ShippingRequestsCarrierDirectPricing");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "ShippingRequestsCarrierDirectPricing");

            migrationBuilder.DropColumn(
                name: "TotalCommission",
                table: "ShippingRequestsCarrierDirectPricing");

            migrationBuilder.DropColumn(
                name: "VatAmount",
                table: "ShippingRequestsCarrierDirectPricing");

            migrationBuilder.AddColumn<decimal>(
                name: "FinalPrice",
                table: "ShippingRequestsCarrierDirectPricing",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinValueComission",
                table: "ShippingRequestsCarrierDirectPricing",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentageComission",
                table: "ShippingRequestsCarrierDirectPricing",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueComission",
                table: "ShippingRequestsCarrierDirectPricing",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
