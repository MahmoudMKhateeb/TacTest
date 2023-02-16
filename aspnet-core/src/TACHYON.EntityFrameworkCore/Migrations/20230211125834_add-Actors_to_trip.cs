using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addActors_to_trip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CarrierActorId",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShipperActorId",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_CarrierActorId",
                table: "ShippingRequestTrips",
                column: "CarrierActorId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_ShipperActorId",
                table: "ShippingRequestTrips",
                column: "ShipperActorId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTrips_Actors_CarrierActorId",
                table: "ShippingRequestTrips",
                column: "CarrierActorId",
                principalTable: "Actors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTrips_Actors_ShipperActorId",
                table: "ShippingRequestTrips",
                column: "ShipperActorId",
                principalTable: "Actors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTrips_Actors_CarrierActorId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTrips_Actors_ShipperActorId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTrips_CarrierActorId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTrips_ShipperActorId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "CarrierActorId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "ShipperActorId",
                table: "ShippingRequestTrips");
        }
    }
}
