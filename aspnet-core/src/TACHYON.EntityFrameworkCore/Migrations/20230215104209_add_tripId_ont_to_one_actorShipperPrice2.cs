using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class add_tripId_ont_to_one_actorShipperPrice2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTrips_ActorShipperPrices_ActorShipperPriceId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTrips_ActorShipperPriceId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "ActorShipperPriceId",
                table: "ShippingRequestTrips");

            migrationBuilder.AddColumn<int>(
                name: "ShippingRequestTripId",
                table: "ActorShipperPrices",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActorShipperPrices_ShippingRequestTripId",
                table: "ActorShipperPrices",
                column: "ShippingRequestTripId",
                unique: true,
                filter: "[ShippingRequestTripId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ActorShipperPrices_ShippingRequestTrips_ShippingRequestTripId",
                table: "ActorShipperPrices",
                column: "ShippingRequestTripId",
                principalTable: "ShippingRequestTrips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActorShipperPrices_ShippingRequestTrips_ShippingRequestTripId",
                table: "ActorShipperPrices");

            migrationBuilder.DropIndex(
                name: "IX_ActorShipperPrices_ShippingRequestTripId",
                table: "ActorShipperPrices");

            migrationBuilder.DropColumn(
                name: "ShippingRequestTripId",
                table: "ActorShipperPrices");

            migrationBuilder.AddColumn<int>(
                name: "ActorShipperPriceId",
                table: "ShippingRequestTrips",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_ActorShipperPriceId",
                table: "ShippingRequestTrips",
                column: "ActorShipperPriceId",
                unique: true,
                filter: "[ActorShipperPriceId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTrips_ActorShipperPrices_ActorShipperPriceId",
                table: "ShippingRequestTrips",
                column: "ActorShipperPriceId",
                principalTable: "ActorShipperPrices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
