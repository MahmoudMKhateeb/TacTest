using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddCommissionPercentageOrAddValueToTrip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CommissionPercentageOrAddValue",
                table: "ShippingRequestTripVases",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionPercentageOrAddValue",
                table: "ShippingRequestTrips",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommissionPercentageOrAddValue",
                table: "ShippingRequestTripVases");

            migrationBuilder.DropColumn(
                name: "CommissionPercentageOrAddValue",
                table: "ShippingRequestTrips");
        }
    }
}
