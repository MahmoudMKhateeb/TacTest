using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddWaybillNumberToShippingRequestTrip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "WaybillNumber",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_WaybillNumber",
                table: "ShippingRequestTrips",
                column: "WaybillNumber",
                unique: true,
                filter: "[WaybillNumber] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTrips_WaybillNumber",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "WaybillNumber",
                table: "ShippingRequestTrips");
        }
    }
}
