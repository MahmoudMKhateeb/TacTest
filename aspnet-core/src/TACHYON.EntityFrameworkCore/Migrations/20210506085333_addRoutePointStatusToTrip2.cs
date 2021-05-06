using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addRoutePointStatusToTrip2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoutPointStatus",
                table: "ShippingRequestTrips");

            migrationBuilder.AddColumn<byte>(
                name: "RoutePointStatus",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoutePointStatus",
                table: "ShippingRequestTrips");

            migrationBuilder.AddColumn<byte>(
                name: "RoutPointStatus",
                table: "ShippingRequestTrips",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
