using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Add_IsWaslIntegrated_to_ShippingRequestTrip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsWaslIntegrated",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "WaslIntegrationId",
                table: "PlateTypes",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsWaslIntegrated",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "WaslIntegrationId",
                table: "PlateTypes");
        }
    }
}