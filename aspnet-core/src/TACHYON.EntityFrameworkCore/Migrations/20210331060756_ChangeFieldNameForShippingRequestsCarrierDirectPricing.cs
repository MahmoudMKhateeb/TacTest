using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class ChangeFieldNameForShippingRequestsCarrierDirectPricing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountSubTotal",
                table: "ShippingRequestsCarrierDirectPricing");

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAmount",
                table: "ShippingRequestsCarrierDirectPricing",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubTotalAmount",
                table: "ShippingRequestsCarrierDirectPricing");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountSubTotal",
                table: "ShippingRequestsCarrierDirectPricing",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
