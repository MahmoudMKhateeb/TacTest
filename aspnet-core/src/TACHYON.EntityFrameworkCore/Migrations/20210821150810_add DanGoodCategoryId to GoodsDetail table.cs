using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addDanGoodCategoryIdtoGoodsDetailtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DanGoodCategoryId",
                table: "GoodsDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GoodsDetails_DanGoodCategoryId",
                table: "GoodsDetails",
                column: "DanGoodCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsDetails_GoodCategories_DanGoodCategoryId",
                table: "GoodsDetails",
                column: "DanGoodCategoryId",
                principalTable: "GoodCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GoodsDetails_GoodCategories_DanGoodCategoryId",
                table: "GoodsDetails");

            migrationBuilder.DropIndex(
                name: "IX_GoodsDetails_DanGoodCategoryId",
                table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "DanGoodCategoryId",
                table: "GoodsDetails");
        }
    }
}
