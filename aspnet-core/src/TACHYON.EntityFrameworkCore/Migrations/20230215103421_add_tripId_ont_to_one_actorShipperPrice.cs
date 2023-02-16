using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class add_tripId_ont_to_one_actorShipperPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActorShipperPrices_ShippingRequestTrips_ShippingRequestTripId",
                table: "ActorShipperPrices");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTrips_ActorShipperPriceId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropIndex(
                name: "IX_ActorShipperPrices_ShippingRequestTripId",
                table: "ActorShipperPrices");

            migrationBuilder.DropColumn(
                name: "ShippingRequestTripId",
                table: "ActorShipperPrices");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_ActorShipperPriceId",
                table: "ShippingRequestTrips",
                column: "ActorShipperPriceId",
                unique: true,
                filter: "[ActorShipperPriceId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTrips_ActorShipperPriceId",
                table: "ShippingRequestTrips");

            migrationBuilder.AddColumn<int>(
                name: "ShippingRequestTripId",
                table: "ActorShipperPrices",
                type: "int",
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
                name: "FK_ActorShipperPrices_ShippingRequestTrips_ShippingRequestTripId",
                table: "ActorShipperPrices",
                column: "ShippingRequestTripId",
                principalTable: "ShippingRequestTrips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
