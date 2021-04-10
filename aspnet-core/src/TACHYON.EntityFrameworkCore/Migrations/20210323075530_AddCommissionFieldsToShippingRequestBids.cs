using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddCommissionFieldsToShippingRequestBids : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BasePrice",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "CommissionValue",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "MinShippmentCommission",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "PercentageCommission",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "TotalCommission",
                table: "ShippingRequests");

            migrationBuilder.AddColumn<decimal>(
                name: "BiddingCommissionValue",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinShippmentBiddingCommission",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentageBiddingCommission",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "price",
                table: "ShippingRequestBids",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<decimal>(
                name: "BasePrice",
                table: "ShippingRequestBids",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCommission",
                table: "ShippingRequestBids",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "BasePrice",
                table: "ShippingRequestBids");

            migrationBuilder.DropColumn(
                name: "TotalCommission",
                table: "ShippingRequestBids");

            migrationBuilder.AddColumn<decimal>(
                name: "BasePrice",
                table: "ShippingRequests",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionValue",
                table: "ShippingRequests",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinShippmentCommission",
                table: "ShippingRequests",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentageCommission",
                table: "ShippingRequests",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCommission",
                table: "ShippingRequests",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "price",
                table: "ShippingRequestBids",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal));
        }
    }
}
