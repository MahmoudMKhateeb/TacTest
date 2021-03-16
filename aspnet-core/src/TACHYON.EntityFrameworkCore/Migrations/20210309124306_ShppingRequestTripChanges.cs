using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class ShppingRequestTripChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTrips_TripStatuses_TripStatusId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTrips_TripStatusId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "TripStatusId",
                table: "ShippingRequestTrips");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndWorking",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartWorking",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Status",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<string>(
                name: "ReceiverCardIdNumber",
                table: "RoutPoints",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiverEmailAddress",
                table: "RoutPoints",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiverFullName",
                table: "RoutPoints",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReceiverId",
                table: "RoutPoints",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiverPhoneNumber",
                table: "RoutPoints",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndWorking",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "StartWorking",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "ReceiverCardIdNumber",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "ReceiverEmailAddress",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "ReceiverFullName",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "ReceiverId",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "ReceiverPhoneNumber",
                table: "RoutPoints");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "ShippingRequestTrips",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TripStatusId",
                table: "ShippingRequestTrips",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_TripStatusId",
                table: "ShippingRequestTrips",
                column: "TripStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTrips_TripStatuses_TripStatusId",
                table: "ShippingRequestTrips",
                column: "TripStatusId",
                principalTable: "TripStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
