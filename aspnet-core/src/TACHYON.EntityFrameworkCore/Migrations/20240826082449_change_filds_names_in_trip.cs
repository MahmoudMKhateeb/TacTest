using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class change_filds_names_in_trip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewDistance",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "NewDriverCommission",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "NewDriverWorkingHour",
                table: "ShippingRequestTrips");

            migrationBuilder.AddColumn<int>(
                name: "ReplacedDriverCommission",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReplacedDriverDistance",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReplacedDriverWorkingHour",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReplacedDriverCommission",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "ReplacedDriverDistance",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "ReplacedDriverWorkingHour",
                table: "ShippingRequestTrips");

            migrationBuilder.AddColumn<int>(
                name: "NewDistance",
                table: "ShippingRequestTrips",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NewDriverCommission",
                table: "ShippingRequestTrips",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NewDriverWorkingHour",
                table: "ShippingRequestTrips",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
