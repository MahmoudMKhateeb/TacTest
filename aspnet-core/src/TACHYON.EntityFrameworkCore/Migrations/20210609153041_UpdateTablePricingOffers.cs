using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class UpdateTablePricingOffers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestVasesPricing_PriceOffers_PriceOfferId",
                table: "ShippingRequestVasesPricing");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestVasesPricing_PriceOfferId",
                table: "ShippingRequestVasesPricing");

            migrationBuilder.DropColumn(
                name: "PriceOfferId",
                table: "ShippingRequestVasesPricing");

            migrationBuilder.AddColumn<byte>(
                name: "PriceType",
                table: "PriceOfferDetails",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceType",
                table: "PriceOfferDetails");

            migrationBuilder.AddColumn<long>(
                name: "PriceOfferId",
                table: "ShippingRequestVasesPricing",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestVasesPricing_PriceOfferId",
                table: "ShippingRequestVasesPricing",
                column: "PriceOfferId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestVasesPricing_PriceOffers_PriceOfferId",
                table: "ShippingRequestVasesPricing",
                column: "PriceOfferId",
                principalTable: "PriceOffers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
