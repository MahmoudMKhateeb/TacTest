using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class add_trip_to_actorprice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShippingRequestTripId",
                table: "ActorCarrierPrices",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActorCarrierPrices_ShippingRequestTripId",
                table: "ActorCarrierPrices",
                column: "ShippingRequestTripId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActorCarrierPrices_ShippingRequestTrips_ShippingRequestTripId",
                table: "ActorCarrierPrices",
                column: "ShippingRequestTripId",
                principalTable: "ShippingRequestTrips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActorCarrierPrices_ShippingRequestTrips_ShippingRequestTripId",
                table: "ActorCarrierPrices");

            migrationBuilder.DropIndex(
                name: "IX_ActorCarrierPrices_ShippingRequestTripId",
                table: "ActorCarrierPrices");

            migrationBuilder.DropColumn(
                name: "ShippingRequestTripId",
                table: "ActorCarrierPrices");
        }
    }
}
