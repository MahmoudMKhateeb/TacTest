using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace TACHYON.Migrations
{
    public partial class Add_Point_TO_Facility_And_Point : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GoodsDetails_GoodCategories_GoodCategoryId",
                table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "NumberOfPackingType",
                table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "PackingType",
                table: "GoodsDetails");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfPackingType",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PackingType",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Amount",
                table: "RoutPointGoodsDetails",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<Point>(
                name: "Location",
                table: "Ports",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Weight",
                table: "GoodsDetails",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GoodCategoryId",
                table: "GoodsDetails",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<Point>(
                name: "Location",
                table: "Facilities",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsDetails_GoodCategories_GoodCategoryId",
                table: "GoodsDetails",
                column: "GoodCategoryId",
                principalTable: "GoodCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GoodsDetails_GoodCategories_GoodCategoryId",
                table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "NumberOfPackingType",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "PackingType",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Facilities");

            migrationBuilder.AlterColumn<double>(
                name: "Amount",
                table: "RoutPointGoodsDetails",
                type: "float",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "Weight",
                table: "GoodsDetails",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<int>(
                name: "GoodCategoryId",
                table: "GoodsDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "NumberOfPackingType",
                table: "GoodsDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PackingType",
                table: "GoodsDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsDetails_GoodCategories_GoodCategoryId",
                table: "GoodsDetails",
                column: "GoodCategoryId",
                principalTable: "GoodCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
