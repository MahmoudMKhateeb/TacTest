using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class CommissionFieldsUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TachyonDealerCommissionValue",
                table: "TachyonPriceOffers");

            migrationBuilder.DropColumn(
                name: "TachyonDealerMinCommissionValue",
                table: "TachyonPriceOffers");

            migrationBuilder.DropColumn(
                name: "TachyonDealerPercentCommission",
                table: "TachyonPriceOffers");

            migrationBuilder.DropColumn(
                name: "BiddingCommissionValue",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "MinShippmentBiddingCommission",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "PercentageBiddingCommission",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "TachyonDealerCommissionValue",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "TachyonDealerMinValueCommission",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "TachyonDealerPercentCommission",
                table: "ShippingRequests");

            migrationBuilder.AddColumn<decimal>(
                name: "ActualCommissionValue",
                table: "TachyonPriceOffers",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ActualMinCommissionValue",
                table: "TachyonPriceOffers",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ActualPercentCommission",
                table: "TachyonPriceOffers",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCommission",
                table: "TachyonPriceOffers",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalCommission",
                table: "ShippingRequests",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TachyonDealerProfit",
                table: "ShippingRequests",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CarrierPrice",
                table: "ShippingRequests",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ActualCommissionValue",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ActualMinCommissionValue",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ActualPercentCommission",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionValueSetting",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MinValueCommissionSetting",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentCommissionSetting",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VatSetting",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ActualCommissionValue",
                table: "ShippingRequestBids",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ActualMinCommissionValue",
                table: "ShippingRequestBids",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ActualPercentCommission",
                table: "ShippingRequestBids",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualCommissionValue",
                table: "TachyonPriceOffers");

            migrationBuilder.DropColumn(
                name: "ActualMinCommissionValue",
                table: "TachyonPriceOffers");

            migrationBuilder.DropColumn(
                name: "ActualPercentCommission",
                table: "TachyonPriceOffers");

            migrationBuilder.DropColumn(
                name: "TotalCommission",
                table: "TachyonPriceOffers");

            migrationBuilder.DropColumn(
                name: "ActualCommissionValue",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "ActualMinCommissionValue",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "ActualPercentCommission",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "CommissionValueSetting",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "MinValueCommissionSetting",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "PercentCommissionSetting",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "VatSetting",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "ActualCommissionValue",
                table: "ShippingRequestBids");

            migrationBuilder.DropColumn(
                name: "ActualMinCommissionValue",
                table: "ShippingRequestBids");

            migrationBuilder.DropColumn(
                name: "ActualPercentCommission",
                table: "ShippingRequestBids");

            migrationBuilder.AddColumn<decimal>(
                name: "TachyonDealerCommissionValue",
                table: "TachyonPriceOffers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TachyonDealerMinCommissionValue",
                table: "TachyonPriceOffers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TachyonDealerPercentCommission",
                table: "TachyonPriceOffers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalCommission",
                table: "ShippingRequests",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "TachyonDealerProfit",
                table: "ShippingRequests",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "CarrierPrice",
                table: "ShippingRequests",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal));

            migrationBuilder.AddColumn<decimal>(
                name: "BiddingCommissionValue",
                table: "ShippingRequests",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinShippmentBiddingCommission",
                table: "ShippingRequests",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentageBiddingCommission",
                table: "ShippingRequests",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TachyonDealerCommissionValue",
                table: "ShippingRequests",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TachyonDealerMinValueCommission",
                table: "ShippingRequests",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TachyonDealerPercentCommission",
                table: "ShippingRequests",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
