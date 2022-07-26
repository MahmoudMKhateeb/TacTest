using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class SplitInvoiceFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SplitInvoiceFlag",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SplitInvoiceFlag",
                table: "ShippingRequests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SplitInvoiceFlag",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "SplitInvoiceFlag",
                table: "ShippingRequests");
        }
    }
}
