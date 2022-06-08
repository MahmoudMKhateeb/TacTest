using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddSupposedPickupTimeRange_shippingRequestTripDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SupposedPickupDateFrom",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SupposedPickupDateTo",
                table: "ShippingRequestTrips",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupposedPickupDateFrom",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "SupposedPickupDateTo",
                table: "ShippingRequestTrips");
        }
    }
}
