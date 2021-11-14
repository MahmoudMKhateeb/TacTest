using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class update_priceOffer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DetailsTotalPricePreCommissionPreVat",
                table: "PriceOffers",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ItemsTotalPricePreCommissionPreVat",
                table: "PriceOffers",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ItemsTotalPricePreCommissionPreVat",
                table: "PriceOfferDetails",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DetailsTotalPricePreCommissionPreVat",
                table: "PriceOffers");

            migrationBuilder.DropColumn(
                name: "ItemsTotalPricePreCommissionPreVat",
                table: "PriceOffers");

            migrationBuilder.DropColumn(
                name: "ItemsTotalPricePreCommissionPreVat",
                table: "PriceOfferDetails");
        }
    }
}