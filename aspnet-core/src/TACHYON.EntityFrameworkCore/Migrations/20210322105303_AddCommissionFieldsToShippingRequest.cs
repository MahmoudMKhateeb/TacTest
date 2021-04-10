using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddCommissionFieldsToShippingRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BasePrice",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionValue",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinShippmentCommission",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentageCommission",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCommission",
                table: "ShippingRequests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
