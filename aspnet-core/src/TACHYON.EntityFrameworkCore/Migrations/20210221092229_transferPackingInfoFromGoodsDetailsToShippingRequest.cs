using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class transferPackingInfoFromGoodsDetailsToShippingRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfPacking",
                table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "PackingType",
                table: "GoodsDetails");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfPacking",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PackingType",
                table: "ShippingRequests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfPacking",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "PackingType",
                table: "ShippingRequests");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfPacking",
                table: "GoodsDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PackingType",
                table: "GoodsDetails",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
