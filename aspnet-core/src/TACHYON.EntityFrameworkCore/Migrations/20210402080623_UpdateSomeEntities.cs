using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class UpdateSomeEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OfferedPrice",
                table: "TachyonPriceOffers");

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
                name: "TaxVat",
                table: "ShippingRequestsCarrierDirectPricing");

            migrationBuilder.DropColumn(
                name: "IsRejected",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "PriceSubTotal",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "TachyonDealerProfit",
                table: "ShippingRequests");

            migrationBuilder.AddColumn<decimal>(
                name: "CarrierPrice",
                table: "TachyonPriceOffers",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionValueSetting",
                table: "TachyonPriceOffers",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MinCommissionValueSetting",
                table: "TachyonPriceOffers",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentCommissionSetting",
                table: "TachyonPriceOffers",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAmount",
                table: "TachyonPriceOffers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantCarrirerId",
                table: "TachyonPriceOffers",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "TachyonPriceOffers",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VatAmount",
                table: "TachyonPriceOffers",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VatSetting",
                table: "TachyonPriceOffers",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ActualCommissionValue",
                table: "ShippingRequestsCarrierDirectPricing",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ActualMinCommissionValue",
                table: "ShippingRequestsCarrierDirectPricing",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ActualPercentCommission",
                table: "ShippingRequestsCarrierDirectPricing",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionValueSetting",
                table: "ShippingRequestsCarrierDirectPricing",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinCommissionValueSetting",
                table: "ShippingRequestsCarrierDirectPricing",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentCommissionSetting",
                table: "ShippingRequestsCarrierDirectPricing",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VatSetting",
                table: "ShippingRequestsCarrierDirectPricing",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAmount",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_TachyonPriceOffers_TenantCarrirerId",
                table: "TachyonPriceOffers",
                column: "TenantCarrirerId");

            migrationBuilder.AddForeignKey(
                name: "FK_TachyonPriceOffers_AbpTenants_TenantCarrirerId",
                table: "TachyonPriceOffers",
                column: "TenantCarrirerId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TachyonPriceOffers_AbpTenants_TenantCarrirerId",
                table: "TachyonPriceOffers");

            migrationBuilder.DropIndex(
                name: "IX_TachyonPriceOffers_TenantCarrirerId",
                table: "TachyonPriceOffers");

            migrationBuilder.DropColumn(
                name: "CarrierPrice",
                table: "TachyonPriceOffers");

            migrationBuilder.DropColumn(
                name: "CommissionValueSetting",
                table: "TachyonPriceOffers");

            migrationBuilder.DropColumn(
                name: "MinCommissionValueSetting",
                table: "TachyonPriceOffers");

            migrationBuilder.DropColumn(
                name: "PercentCommissionSetting",
                table: "TachyonPriceOffers");

            migrationBuilder.DropColumn(
                name: "SubTotalAmount",
                table: "TachyonPriceOffers");

            migrationBuilder.DropColumn(
                name: "TenantCarrirerId",
                table: "TachyonPriceOffers");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "TachyonPriceOffers");

            migrationBuilder.DropColumn(
                name: "VatAmount",
                table: "TachyonPriceOffers");

            migrationBuilder.DropColumn(
                name: "VatSetting",
                table: "TachyonPriceOffers");

            migrationBuilder.DropColumn(
                name: "ActualCommissionValue",
                table: "ShippingRequestsCarrierDirectPricing");

            migrationBuilder.DropColumn(
                name: "ActualMinCommissionValue",
                table: "ShippingRequestsCarrierDirectPricing");

            migrationBuilder.DropColumn(
                name: "ActualPercentCommission",
                table: "ShippingRequestsCarrierDirectPricing");

            migrationBuilder.DropColumn(
                name: "CommissionValueSetting",
                table: "ShippingRequestsCarrierDirectPricing");

            migrationBuilder.DropColumn(
                name: "MinCommissionValueSetting",
                table: "ShippingRequestsCarrierDirectPricing");

            migrationBuilder.DropColumn(
                name: "PercentCommissionSetting",
                table: "ShippingRequestsCarrierDirectPricing");

            migrationBuilder.DropColumn(
                name: "VatSetting",
                table: "ShippingRequestsCarrierDirectPricing");

            migrationBuilder.DropColumn(
                name: "SubTotalAmount",
                table: "ShippingRequests");

            migrationBuilder.AddColumn<decimal>(
                name: "OfferedPrice",
                table: "TachyonPriceOffers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionValue",
                table: "ShippingRequestsCarrierDirectPricing",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinCommissionValue",
                table: "ShippingRequestsCarrierDirectPricing",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentCommission",
                table: "ShippingRequestsCarrierDirectPricing",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SettingsCommissionValue",
                table: "ShippingRequestsCarrierDirectPricing",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SettingsMinCommissionValue",
                table: "ShippingRequestsCarrierDirectPricing",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SettingsPercentCommission",
                table: "ShippingRequestsCarrierDirectPricing",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxVat",
                table: "ShippingRequestsCarrierDirectPricing",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRejected",
                table: "ShippingRequests",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceSubTotal",
                table: "ShippingRequests",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TachyonDealerProfit",
                table: "ShippingRequests",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
