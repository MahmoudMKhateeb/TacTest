using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addTotalbidToShipping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiverFkId",
                table: "RoutPoints");

            migrationBuilder.AddColumn<int>(
                name: "TotalBids",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_RejectReasonId",
                table: "ShippingRequestTrips",
                column: "RejectReasonId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTrips_ShippingRequestTripRejectReasons_RejectReasonId",
                table: "ShippingRequestTrips",
                column: "RejectReasonId",
                principalTable: "ShippingRequestTripRejectReasons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTrips_ShippingRequestTripRejectReasons_RejectReasonId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTrips_RejectReasonId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "TotalBids",
                table: "ShippingRequests");

            migrationBuilder.AddColumn<long>(
                name: "ReceiverFkId",
                table: "RoutPoints",
                type: "bigint",
                nullable: true);
        }
    }
}
