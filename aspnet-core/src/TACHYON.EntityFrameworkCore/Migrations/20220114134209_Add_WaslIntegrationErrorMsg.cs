using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Add_WaslIntegrationErrorMsg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WaslIntegrationErrorMsg",
                table: "Trucks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WaslIntegrationErrorMsg",
                table: "ShippingRequestTrips",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WaslIntegrationErrorMsg",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "WaslIntegrationErrorMsg",
                table: "ShippingRequestTrips");
        }
    }
}
