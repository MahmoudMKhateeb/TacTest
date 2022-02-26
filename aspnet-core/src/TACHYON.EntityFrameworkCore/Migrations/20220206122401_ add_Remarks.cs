using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class add_Remarks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanBePrinted",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ContainerNumber",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoundTrip",
                table: "ShippingRequestTrips",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanBePrinted",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "ContainerNumber",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "RoundTrip",
                table: "ShippingRequestTrips");
        }
    }
}
