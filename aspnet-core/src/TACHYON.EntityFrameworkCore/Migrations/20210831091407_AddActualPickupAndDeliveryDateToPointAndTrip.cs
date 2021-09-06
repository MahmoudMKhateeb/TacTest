using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddActualPickupAndDeliveryDateToPointAndTrip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ActualDeliveryDate",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualPickupDate",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualPickupOrDeliveryDate",
                table: "RoutPoints",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualDeliveryDate",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "ActualPickupDate",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "ActualPickupOrDeliveryDate",
                table: "RoutPoints");
        }
    }
}
