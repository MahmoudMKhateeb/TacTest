using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class add_city_fk_to_price_package_offer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DestinationCityId",
                table: "PricePackageOffers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OriginCityId",
                table: "PricePackageOffers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PricePackageOffers_DestinationCityId",
                table: "PricePackageOffers",
                column: "DestinationCityId");

            migrationBuilder.CreateIndex(
                name: "IX_PricePackageOffers_OriginCityId",
                table: "PricePackageOffers",
                column: "OriginCityId");

            migrationBuilder.AddForeignKey(
                name: "FK_PricePackageOffers_Cities_DestinationCityId",
                table: "PricePackageOffers",
                column: "DestinationCityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PricePackageOffers_Cities_OriginCityId",
                table: "PricePackageOffers",
                column: "OriginCityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PricePackageOffers_Cities_DestinationCityId",
                table: "PricePackageOffers");

            migrationBuilder.DropForeignKey(
                name: "FK_PricePackageOffers_Cities_OriginCityId",
                table: "PricePackageOffers");

            migrationBuilder.DropIndex(
                name: "IX_PricePackageOffers_DestinationCityId",
                table: "PricePackageOffers");

            migrationBuilder.DropIndex(
                name: "IX_PricePackageOffers_OriginCityId",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "DestinationCityId",
                table: "PricePackageOffers");

            migrationBuilder.DropColumn(
                name: "OriginCityId",
                table: "PricePackageOffers");
        }
    }
}