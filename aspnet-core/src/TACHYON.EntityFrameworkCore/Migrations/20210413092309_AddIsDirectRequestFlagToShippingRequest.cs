using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddIsDirectRequestFlagToShippingRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShippingRequestCarrierDirectPricingId",
                table: "TachyonPriceOffers",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDirectRequest",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_TachyonPriceOffers_ShippingRequestCarrierDirectPricingId",
                table: "TachyonPriceOffers",
                column: "ShippingRequestCarrierDirectPricingId");

            migrationBuilder.AddForeignKey(
                name: "FK_TachyonPriceOffers_ShippingRequestsCarrierDirectPricing_ShippingRequestCarrierDirectPricingId",
                table: "TachyonPriceOffers",
                column: "ShippingRequestCarrierDirectPricingId",
                principalTable: "ShippingRequestsCarrierDirectPricing",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TachyonPriceOffers_ShippingRequestsCarrierDirectPricing_ShippingRequestCarrierDirectPricingId",
                table: "TachyonPriceOffers");

            migrationBuilder.DropIndex(
                name: "IX_TachyonPriceOffers_ShippingRequestCarrierDirectPricingId",
                table: "TachyonPriceOffers");

            migrationBuilder.DropColumn(
                name: "ShippingRequestCarrierDirectPricingId",
                table: "TachyonPriceOffers");

            migrationBuilder.DropColumn(
                name: "IsDirectRequest",
                table: "ShippingRequests");
        }
    }
}
