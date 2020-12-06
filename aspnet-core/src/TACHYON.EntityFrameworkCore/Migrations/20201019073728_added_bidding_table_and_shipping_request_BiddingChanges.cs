using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class added_bidding_table_and_shipping_request_BiddingChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BidEndDate",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BidStartDate",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsClosedBid",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isCancledBid",
                table: "ShippingRequests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BidEndDate",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "BidStartDate",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "IsClosedBid",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "isCancledBid",
                table: "ShippingRequests");
        }
    }
}
