using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class SetShippingRequesCarrierPriceTypeAsNotNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "CarrierPriceType",
                table: "ShippingRequests",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "CarrierPriceType",
                table: "ShippingRequests",
                type: "tinyint",
                nullable: true,
                oldClrType: typeof(byte));
        }
    }
}
