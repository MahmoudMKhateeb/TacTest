using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddTachyonMSCanceledInTrip_field : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApproveCancledByTachyonDealer",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsForcedCanceledByTachyonDealer",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproveCancledByTachyonDealer",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "IsForcedCanceledByTachyonDealer",
                table: "ShippingRequestTrips");
        }
    }
}