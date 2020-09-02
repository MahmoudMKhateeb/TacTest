using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addGoodCategoryIdtoShippingRequesttable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GoodCategoryId",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_GoodCategoryId",
                table: "ShippingRequests",
                column: "GoodCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_GoodCategories_GoodCategoryId",
                table: "ShippingRequests",
                column: "GoodCategoryId",
                principalTable: "GoodCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_GoodCategories_GoodCategoryId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_GoodCategoryId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "GoodCategoryId",
                table: "ShippingRequests");
        }
    }
}
