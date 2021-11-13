using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Add_DetailsTotalPricePostCommissionPreVat_ItemsTotalPricePostCommissionPreVat_To_PriceOffer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DetailsTotalPricePostCommissionPreVat",
                table: "PriceOffers",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ItemsTotalPricePostCommissionPreVat",
                table: "PriceOffers",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DetailsTotalPricePostCommissionPreVat",
                table: "PriceOffers");

            migrationBuilder.DropColumn(
                name: "ItemsTotalPricePostCommissionPreVat",
                table: "PriceOffers");
        }
    }
}
