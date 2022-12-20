using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class release_2_4_22 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShippingTypeId",
                table: "TmsPricePackages",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TmsPricePackages_ShippingTypeId",
                table: "TmsPricePackages",
                column: "ShippingTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TmsPricePackages_ShippingTypes_ShippingTypeId",
                table: "TmsPricePackages",
                column: "ShippingTypeId",
                principalTable: "ShippingTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TmsPricePackages_ShippingTypes_ShippingTypeId",
                table: "TmsPricePackages");

            migrationBuilder.DropIndex(
                name: "IX_TmsPricePackages_ShippingTypeId",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "ShippingTypeId",
                table: "TmsPricePackages");
        }
    }
}
