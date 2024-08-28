using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Add_ReplacedTruckId_to_Trip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ReplacedTruckId",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_ReplacedTruckId",
                table: "ShippingRequestTrips",
                column: "ReplacedTruckId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTrips_Trucks_ReplacedTruckId",
                table: "ShippingRequestTrips",
                column: "ReplacedTruckId",
                principalTable: "Trucks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTrips_Trucks_ReplacedTruckId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTrips_ReplacedTruckId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "ReplacedTruckId",
                table: "ShippingRequestTrips");
        }
    }
}
