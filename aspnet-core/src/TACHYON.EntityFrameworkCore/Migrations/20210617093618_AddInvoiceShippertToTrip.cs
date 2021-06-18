using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddInvoiceShippertToTrip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCarrierHaveInvoice",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsShipperHaveInvoice",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCarrierHaveInvoice",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "IsShipperHaveInvoice",
                table: "ShippingRequestTrips");
        }
    }
}
