using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class release_2_4_14 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DedicatedShippingRequestDrivers_AbpUsers_DriverUserId",
                table: "DedicatedShippingRequestDrivers");

            migrationBuilder.AddColumn<bool>(
                name: "IsRequestedToReplace",
                table: "DedicatedShippingRequestTrucks",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "OriginalDedicatedTruckId",
                table: "DedicatedShippingRequestTrucks",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReplacementDate",
                table: "DedicatedShippingRequestTrucks",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "ReplacementFlag",
                table: "DedicatedShippingRequestTrucks",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "ReplacementIntervalInDays",
                table: "DedicatedShippingRequestTrucks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReplacementReason",
                table: "DedicatedShippingRequestTrucks",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRequestedToReplace",
                table: "DedicatedShippingRequestDrivers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "OriginalDedicatedDriverId",
                table: "DedicatedShippingRequestDrivers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReplacementDate",
                table: "DedicatedShippingRequestDrivers",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "ReplacementFlag",
                table: "DedicatedShippingRequestDrivers",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "ReplacementIntervalInDays",
                table: "DedicatedShippingRequestDrivers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReplacementReason",
                table: "DedicatedShippingRequestDrivers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedShippingRequestTrucks_OriginalDedicatedTruckId",
                table: "DedicatedShippingRequestTrucks",
                column: "OriginalDedicatedTruckId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedShippingRequestDrivers_OriginalDedicatedDriverId",
                table: "DedicatedShippingRequestDrivers",
                column: "OriginalDedicatedDriverId");

            migrationBuilder.AddForeignKey(
                name: "FK_DedicatedShippingRequestDrivers_AbpUsers_DriverUserId",
                table: "DedicatedShippingRequestDrivers",
                column: "DriverUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DedicatedShippingRequestDrivers_DedicatedShippingRequestDrivers_OriginalDedicatedDriverId",
                table: "DedicatedShippingRequestDrivers",
                column: "OriginalDedicatedDriverId",
                principalTable: "DedicatedShippingRequestDrivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DedicatedShippingRequestTrucks_DedicatedShippingRequestTrucks_OriginalDedicatedTruckId",
                table: "DedicatedShippingRequestTrucks",
                column: "OriginalDedicatedTruckId",
                principalTable: "DedicatedShippingRequestTrucks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DedicatedShippingRequestDrivers_AbpUsers_DriverUserId",
                table: "DedicatedShippingRequestDrivers");

            migrationBuilder.DropForeignKey(
                name: "FK_DedicatedShippingRequestDrivers_DedicatedShippingRequestDrivers_OriginalDedicatedDriverId",
                table: "DedicatedShippingRequestDrivers");

            migrationBuilder.DropForeignKey(
                name: "FK_DedicatedShippingRequestTrucks_DedicatedShippingRequestTrucks_OriginalDedicatedTruckId",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropIndex(
                name: "IX_DedicatedShippingRequestTrucks_OriginalDedicatedTruckId",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropIndex(
                name: "IX_DedicatedShippingRequestDrivers_OriginalDedicatedDriverId",
                table: "DedicatedShippingRequestDrivers");

            migrationBuilder.DropColumn(
                name: "IsRequestedToReplace",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropColumn(
                name: "OriginalDedicatedTruckId",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropColumn(
                name: "ReplacementDate",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropColumn(
                name: "ReplacementFlag",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropColumn(
                name: "ReplacementIntervalInDays",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropColumn(
                name: "ReplacementReason",
                table: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropColumn(
                name: "IsRequestedToReplace",
                table: "DedicatedShippingRequestDrivers");

            migrationBuilder.DropColumn(
                name: "OriginalDedicatedDriverId",
                table: "DedicatedShippingRequestDrivers");

            migrationBuilder.DropColumn(
                name: "ReplacementDate",
                table: "DedicatedShippingRequestDrivers");

            migrationBuilder.DropColumn(
                name: "ReplacementFlag",
                table: "DedicatedShippingRequestDrivers");

            migrationBuilder.DropColumn(
                name: "ReplacementIntervalInDays",
                table: "DedicatedShippingRequestDrivers");

            migrationBuilder.DropColumn(
                name: "ReplacementReason",
                table: "DedicatedShippingRequestDrivers");

            migrationBuilder.AddForeignKey(
                name: "FK_DedicatedShippingRequestDrivers_AbpUsers_DriverUserId",
                table: "DedicatedShippingRequestDrivers",
                column: "DriverUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
