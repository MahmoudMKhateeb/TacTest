using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class add_tripId_ont_to_one_actorShipperPrice4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTrips_ActorCarrierPrices_ActorCarrierPriceId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTrips_ActorShipperPrices_ActorShipperPriceId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTrips_ActorCarrierPriceId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTrips_ActorShipperPriceId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropIndex(
                name: "IX_ActorShipperPrices_ShippingRequestTripId",
                table: "ActorShipperPrices");

            migrationBuilder.DropIndex(
                name: "IX_ActorCarrierPrices_ShippingRequestTripId",
                table: "ActorCarrierPrices");

            migrationBuilder.DropColumn(
                name: "ActorCarrierPriceId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "ActorShipperPriceId",
                table: "ShippingRequestTrips");

            migrationBuilder.CreateIndex(
                name: "IX_ActorShipperPrices_ShippingRequestTripId",
                table: "ActorShipperPrices",
                column: "ShippingRequestTripId",
                unique: true,
                filter: "[ShippingRequestTripId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ActorCarrierPrices_ShippingRequestTripId",
                table: "ActorCarrierPrices",
                column: "ShippingRequestTripId",
                unique: true,
                filter: "[ShippingRequestTripId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ActorShipperPrices_ShippingRequestTripId",
                table: "ActorShipperPrices");

            migrationBuilder.DropIndex(
                name: "IX_ActorCarrierPrices_ShippingRequestTripId",
                table: "ActorCarrierPrices");

            migrationBuilder.AddColumn<int>(
                name: "ActorCarrierPriceId",
                table: "ShippingRequestTrips",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ActorShipperPriceId",
                table: "ShippingRequestTrips",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_ActorCarrierPriceId",
                table: "ShippingRequestTrips",
                column: "ActorCarrierPriceId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_ActorShipperPriceId",
                table: "ShippingRequestTrips",
                column: "ActorShipperPriceId");

            migrationBuilder.CreateIndex(
                name: "IX_ActorShipperPrices_ShippingRequestTripId",
                table: "ActorShipperPrices",
                column: "ShippingRequestTripId");

            migrationBuilder.CreateIndex(
                name: "IX_ActorCarrierPrices_ShippingRequestTripId",
                table: "ActorCarrierPrices",
                column: "ShippingRequestTripId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTrips_ActorCarrierPrices_ActorCarrierPriceId",
                table: "ShippingRequestTrips",
                column: "ActorCarrierPriceId",
                principalTable: "ActorCarrierPrices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
