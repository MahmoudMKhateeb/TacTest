using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class tripaddconfirmcancel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproveCancledByCarrier",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "IsApproveCancledByShipper",
                table: "ShippingRequests");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproveCancledByCarrier",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproveCancledByShipper",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproveCancledByCarrier",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "IsApproveCancledByShipper",
                table: "ShippingRequestTrips");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproveCancledByCarrier",
                table: "ShippingRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproveCancledByShipper",
                table: "ShippingRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
