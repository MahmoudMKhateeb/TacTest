using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class UpdateTablePricingOffer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CommissionPercentageOrAddValue",
                table: "ShippingRequestVasesPricing",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<byte>(
                name: "CommissionType",
                table: "ShippingRequestVasesPricing",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommissionPercentageOrAddValue",
                table: "ShippingRequestVasesPricing");

            migrationBuilder.DropColumn(
                name: "CommissionType",
                table: "ShippingRequestVasesPricing");
        }
    }
}
