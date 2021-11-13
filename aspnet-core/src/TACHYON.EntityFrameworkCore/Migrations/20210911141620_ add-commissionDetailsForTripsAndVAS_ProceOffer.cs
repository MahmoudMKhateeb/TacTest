using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addcommissionDetailsForTripsAndVAS_ProceOffer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DetailsTotalCommission",
                table: "PriceOffers",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ItemsTotalCommission",
                table: "PriceOffers",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DetailsTotalCommission",
                table: "PriceOffers");

            migrationBuilder.DropColumn(
                name: "ItemsTotalCommission",
                table: "PriceOffers");
        }
    }
}
