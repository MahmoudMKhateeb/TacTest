using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class FixaddDanGoodCategoryIdtoGoodsDetailtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "DangerousGoodTypeId",
                table: "GoodsDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GoodsDetails_DangerousGoodTypeId",
                table: "GoodsDetails",
                column: "DangerousGoodTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsDetails_DangerousGoodTypes_DangerousGoodTypeId",
                table: "GoodsDetails",
                column: "DangerousGoodTypeId",
                principalTable: "DangerousGoodTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GoodsDetails_DangerousGoodTypes_DangerousGoodTypeId",
                table: "GoodsDetails");

            migrationBuilder.DropIndex(
                name: "IX_GoodsDetails_DangerousGoodTypeId",
                table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "DangerousGoodTypeId",
                table: "GoodsDetails");

            migrationBuilder.AddColumn<int>(
                name: "DanGoodCategoryId",
                table: "GoodsDetails",
                type: "int",
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
    }
}
