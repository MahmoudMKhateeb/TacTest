using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddCancelingDetails_ShippingRequestTripTB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "CancelStatus",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<string>(
                name: "RejectedCancelingReason",
                table: "ShippingRequestTrips",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelStatus",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "RejectedCancelingReason",
                table: "ShippingRequestTrips");
        }
    }
}
