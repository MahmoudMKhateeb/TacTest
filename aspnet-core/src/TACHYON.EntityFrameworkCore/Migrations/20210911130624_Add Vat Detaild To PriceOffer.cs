using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddVatDetaildToPriceOffer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DetailsTotalVatAmountPreCommission",
                table: "PriceOffers",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ItemsTotalVatAmountPreCommission",
                table: "PriceOffers",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DetailsTotalVatAmountPreCommission",
                table: "PriceOffers");

            migrationBuilder.DropColumn(
                name: "ItemsTotalVatAmountPreCommission",
                table: "PriceOffers");
        }
    }
}
