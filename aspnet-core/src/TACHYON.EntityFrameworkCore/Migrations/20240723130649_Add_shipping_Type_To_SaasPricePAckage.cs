using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Add_shipping_Type_To_SaasPricePAckage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GoodCategoryId",
                table: "SaasPricePackages",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShippingTypeId",
                table: "SaasPricePackages",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaasPricePackages_GoodCategoryId",
                table: "SaasPricePackages",
                column: "GoodCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaasPricePackages_GoodCategories_GoodCategoryId",
                table: "SaasPricePackages",
                column: "GoodCategoryId",
                principalTable: "GoodCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaasPricePackages_GoodCategories_GoodCategoryId",
                table: "SaasPricePackages");

            migrationBuilder.DropIndex(
                name: "IX_SaasPricePackages_GoodCategoryId",
                table: "SaasPricePackages");

            migrationBuilder.DropColumn(
                name: "GoodCategoryId",
                table: "SaasPricePackages");

            migrationBuilder.DropColumn(
                name: "ShippingTypeId",
                table: "SaasPricePackages");
        }
    }
}
