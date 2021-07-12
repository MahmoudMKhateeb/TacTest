using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Rename_TripNote_to_Note_ShippingRequestTrip_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TripNote",
                table: "ShippingRequestTrips");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "ShippingRequestTrips",
                maxLength: 600,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "ShippingRequestTrips");

            migrationBuilder.AddColumn<string>(
                name: "TripNote",
                table: "ShippingRequestTrips",
                type: "nvarchar(600)",
                maxLength: 600,
                nullable: true);
        }
    }
}
