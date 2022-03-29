using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class edit_trip_fk_inPenalty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Penalties_ShippingRequestTrips_TripFKId",
                table: "Penalties");

            migrationBuilder.DropIndex(
                name: "IX_Penalties_TripFKId",
                table: "Penalties");

            migrationBuilder.DropColumn(
                name: "TripFKId",
                table: "Penalties");

            migrationBuilder.DropColumn(
                name: "TripId",
                table: "Penalties");

            migrationBuilder.AddColumn<int>(
                name: "ShippingRequestTripId",
                table: "Penalties",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Penalties_ShippingRequestTripId",
                table: "Penalties",
                column: "ShippingRequestTripId");

            migrationBuilder.AddForeignKey(
                name: "FK_Penalties_ShippingRequestTrips_ShippingRequestTripId",
                table: "Penalties",
                column: "ShippingRequestTripId",
                principalTable: "ShippingRequestTrips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Penalties_ShippingRequestTrips_ShippingRequestTripId",
                table: "Penalties");

            migrationBuilder.DropIndex(
                name: "IX_Penalties_ShippingRequestTripId",
                table: "Penalties");

            migrationBuilder.DropColumn(
                name: "ShippingRequestTripId",
                table: "Penalties");

            migrationBuilder.AddColumn<int>(
                name: "TripFKId",
                table: "Penalties",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TripId",
                table: "Penalties",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Penalties_TripFKId",
                table: "Penalties",
                column: "TripFKId");

            migrationBuilder.AddForeignKey(
                name: "FK_Penalties_ShippingRequestTrips_TripFKId",
                table: "Penalties",
                column: "TripFKId",
                principalTable: "ShippingRequestTrips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
