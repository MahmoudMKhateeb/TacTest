using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddNewDriverDetailsToTheTrip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DriverCommission",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NewDistance",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NewDriverCommission",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NewDriverWorkingHour",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriverCommission",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "NewDistance",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "NewDriverCommission",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "NewDriverWorkingHour",
                table: "ShippingRequestTrips");
        }
    }
}
