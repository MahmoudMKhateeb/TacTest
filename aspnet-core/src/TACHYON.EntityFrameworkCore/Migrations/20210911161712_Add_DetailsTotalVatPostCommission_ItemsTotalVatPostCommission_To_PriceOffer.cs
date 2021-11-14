using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Add_DetailsTotalVatPostCommission_ItemsTotalVatPostCommission_To_PriceOffer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DetailsTotalVatPostCommission",
                table: "PriceOffers",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ItemsTotalVatPostCommission",
                table: "PriceOffers",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DetailsTotalVatPostCommission",
                table: "PriceOffers");

            migrationBuilder.DropColumn(
                name: "ItemsTotalVatPostCommission",
                table: "PriceOffers");
        }
    }
}