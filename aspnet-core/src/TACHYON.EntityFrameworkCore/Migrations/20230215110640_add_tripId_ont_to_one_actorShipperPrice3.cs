using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class add_tripId_ont_to_one_actorShipperPrice3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ActorShipperPrices_ShippingRequestTripId",
                table: "ActorShipperPrices");

            migrationBuilder.AddColumn<int>(
                name: "ActorShipperPriceId",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_ActorShipperPriceId",
                table: "ShippingRequestTrips",
                column: "ActorShipperPriceId");

            migrationBuilder.CreateIndex(
                name: "IX_ActorShipperPrices_ShippingRequestTripId",
                table: "ActorShipperPrices",
                column: "ShippingRequestTripId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTrips_ActorShipperPrices_ActorShipperPriceId",
                table: "ShippingRequestTrips",
                column: "ActorShipperPriceId",
                principalTable: "ActorShipperPrices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTrips_ActorShipperPrices_ActorShipperPriceId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTrips_ActorShipperPriceId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropIndex(
                name: "IX_ActorShipperPrices_ShippingRequestTripId",
                table: "ActorShipperPrices");

            migrationBuilder.DropColumn(
                name: "ActorShipperPriceId",
                table: "ShippingRequestTrips");

            migrationBuilder.CreateIndex(
                name: "IX_ActorShipperPrices_ShippingRequestTripId",
                table: "ActorShipperPrices",
                column: "ShippingRequestTripId",
                unique: true,
                filter: "[ShippingRequestTripId] IS NOT NULL");
        }
    }
}
