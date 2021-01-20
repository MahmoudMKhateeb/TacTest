using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class UpdateSR : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfPackingType",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "PackingType",
                table: "ShippingRequests");

            migrationBuilder.AlterColumn<double>(
                name: "Weight",
                table: "GoodsDetails",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfPacking",
                table: "GoodsDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PackingType",
                table: "GoodsDetails",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfPacking",
                table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "PackingType",
                table: "GoodsDetails");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfPackingType",
                table: "ShippingRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PackingType",
                table: "ShippingRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Weight",
                table: "GoodsDetails",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(double),
                oldMaxLength: 64);
        }
    }
}
