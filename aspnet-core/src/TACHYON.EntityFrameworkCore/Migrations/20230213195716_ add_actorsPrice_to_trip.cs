using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class add_actorsPrice_to_trip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActorCarrierPriceId",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ActorShipperPriceId",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_ActorCarrierPriceId",
                table: "ShippingRequestTrips",
                column: "ActorCarrierPriceId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_ActorShipperPriceId",
                table: "ShippingRequestTrips",
                column: "ActorShipperPriceId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "ActorCarrierPriceId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "ActorShipperPriceId",
                table: "ShippingRequestTrips");
        }
    }
}
