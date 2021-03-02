using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddShippingRequestIdToShippingRequestTripsEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ShippingRequestId",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_ShippingRequestId",
                table: "ShippingRequestTrips",
                column: "ShippingRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTrips_ShippingRequests_ShippingRequestId",
                table: "ShippingRequestTrips",
                column: "ShippingRequestId",
                principalTable: "ShippingRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTrips_ShippingRequests_ShippingRequestId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTrips_ShippingRequestId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "ShippingRequestId",
                table: "ShippingRequestTrips");
        }
    }
}
