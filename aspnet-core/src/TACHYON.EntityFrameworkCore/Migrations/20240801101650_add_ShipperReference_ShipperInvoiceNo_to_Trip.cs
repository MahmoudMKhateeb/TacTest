using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class add_ShipperReference_ShipperInvoiceNo_to_Trip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShipperInvoiceNo",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShipperReference",
                table: "ShippingRequestTrips",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShipperInvoiceNo",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "ShipperReference",
                table: "ShippingRequestTrips");
        }
    }
}
