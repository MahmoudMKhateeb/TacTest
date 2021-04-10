using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class UpdateShippingRequestsCarrierDirectPricingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "SubTotalAmount",
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

            migrationBuilder.DropColumn(
                name: "VatSetting",
                table: "ShippingRequestsCarrierDirectPricing");

            migrationBuilder.AlterColumn<decimal>(
                name: "SubTotalAmount",
                table: "TachyonPriceOffers",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "SubTotalAmount",
                table: "TachyonPriceOffers",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal));

            migrationBuilder.AddColumn<decimal>(
                name: "ActualCommissionValue",
                table: "ShippingRequestsCarrierDirectPricing",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ActualMinCommissionValue",
                table: "ShippingRequestsCarrierDirectPricing",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ActualPercentCommission",
                table: "ShippingRequestsCarrierDirectPricing",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionValueSetting",
                table: "ShippingRequestsCarrierDirectPricing",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinCommissionValueSetting",
                table: "ShippingRequestsCarrierDirectPricing",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentCommissionSetting",
                table: "ShippingRequestsCarrierDirectPricing",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAmount",
                table: "ShippingRequestsCarrierDirectPricing",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "ShippingRequestsCarrierDirectPricing",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCommission",
                table: "ShippingRequestsCarrierDirectPricing",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VatAmount",
                table: "ShippingRequestsCarrierDirectPricing",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VatSetting",
                table: "ShippingRequestsCarrierDirectPricing",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
