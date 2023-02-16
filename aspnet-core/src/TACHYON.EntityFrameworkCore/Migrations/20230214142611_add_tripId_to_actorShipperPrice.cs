using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class add_tripId_to_actorShipperPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShippingRequestTripId",
                table: "ActorShipperPrices",
                nullable: true);

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
        }
    }
}
