using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addReferenceNumberToShippingRequestPricings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PricingNumber",
                table: "ShippingRequestPricings");

            migrationBuilder.AddColumn<long>(
                name: "ReferenceNumber",
                table: "ShippingRequestPricings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferenceNumber",
                table: "ShippingRequestPricings");

            migrationBuilder.AddColumn<long>(
                name: "PricingNumber",
                table: "ShippingRequestPricings",
                type: "bigint",
                nullable: true);
        }
    }
}
