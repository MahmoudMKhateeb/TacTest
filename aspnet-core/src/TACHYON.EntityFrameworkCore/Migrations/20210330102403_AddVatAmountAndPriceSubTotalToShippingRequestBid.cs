using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddVatAmountAndPriceSubTotalToShippingRequestBid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PriceSubTotal",
                table: "ShippingRequestBids",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VatAmount",
                table: "ShippingRequestBids",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceSubTotal",
                table: "ShippingRequestBids");

            migrationBuilder.DropColumn(
                name: "VatAmount",
                table: "ShippingRequestBids");
        }
    }
}
