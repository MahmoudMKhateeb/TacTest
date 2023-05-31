using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class _3146 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PricePackages_ShippingTypes_ShippingTypeId",
                table: "PricePackages");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestDestinationCities_ShippingRequests_ShippingRequestId",
                table: "ShippingRequestDestinationCities");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTripVases_ShippingRequestVases_ShippingRequestVasId",
                table: "ShippingRequestTripVases");

            migrationBuilder.DropIndex(
                name: "IX_PricePackages_ShippingTypeId",
                table: "PricePackages");

            migrationBuilder.AlterColumn<long>(
                name: "ShippingRequestVasId",
                table: "ShippingRequestTripVases",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<int>(
                name: "VasId",
                table: "ShippingRequestTripVases",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CarrierInvoiceStatus",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OriginCityId",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "RoundTripType",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShippingTypeId",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ShippingRequestId",
                table: "ShippingRequestDestinationCities",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<int>(
                name: "ShippingRequestTripId",
                table: "ShippingRequestDestinationCities",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ShippingTypeId",
                table: "PricePackages",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "RouteType",
                table: "PricePackages",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AddColumn<long>(
                name: "DestinationFacilityPortId",
                table: "PricePackages",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OriginFacilityPortId",
                table: "PricePackages",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "RoundTrip",
                table: "PricePackages",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ConfirmationDate",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ConsiderConfirmationAndLoadingDates",
                table: "Invoices",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTripVases_VasId",
                table: "ShippingRequestTripVases",
                column: "VasId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_OriginCityId",
                table: "ShippingRequestTrips",
                column: "OriginCityId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestDestinationCities_ShippingRequestTripId",
                table: "ShippingRequestDestinationCities",
                column: "ShippingRequestTripId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestDestinationCities_ShippingRequests_ShippingRequestId",
                table: "ShippingRequestDestinationCities",
                column: "ShippingRequestId",
                principalTable: "ShippingRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestDestinationCities_ShippingRequestTrips_ShippingRequestTripId",
                table: "ShippingRequestDestinationCities",
                column: "ShippingRequestTripId",
                principalTable: "ShippingRequestTrips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTrips_Cities_OriginCityId",
                table: "ShippingRequestTrips",
                column: "OriginCityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTripVases_ShippingRequestVases_ShippingRequestVasId",
                table: "ShippingRequestTripVases",
                column: "ShippingRequestVasId",
                principalTable: "ShippingRequestVases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTripVases_Vases_VasId",
                table: "ShippingRequestTripVases",
                column: "VasId",
                principalTable: "Vases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestDestinationCities_ShippingRequests_ShippingRequestId",
                table: "ShippingRequestDestinationCities");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestDestinationCities_ShippingRequestTrips_ShippingRequestTripId",
                table: "ShippingRequestDestinationCities");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTrips_Cities_OriginCityId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTripVases_ShippingRequestVases_ShippingRequestVasId",
                table: "ShippingRequestTripVases");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTripVases_Vases_VasId",
                table: "ShippingRequestTripVases");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTripVases_VasId",
                table: "ShippingRequestTripVases");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTrips_OriginCityId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestDestinationCities_ShippingRequestTripId",
                table: "ShippingRequestDestinationCities");

            migrationBuilder.DropColumn(
                name: "VasId",
                table: "ShippingRequestTripVases");

            migrationBuilder.DropColumn(
                name: "CarrierInvoiceStatus",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "OriginCityId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "RoundTripType",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "ShippingTypeId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "ShippingRequestTripId",
                table: "ShippingRequestDestinationCities");

            migrationBuilder.DropColumn(
                name: "DestinationFacilityPortId",
                table: "PricePackages");

            migrationBuilder.DropColumn(
                name: "OriginFacilityPortId",
                table: "PricePackages");

            migrationBuilder.DropColumn(
                name: "RoundTrip",
                table: "PricePackages");

            migrationBuilder.DropColumn(
                name: "ConfirmationDate",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ConsiderConfirmationAndLoadingDates",
                table: "Invoices");

            migrationBuilder.AlterColumn<long>(
                name: "ShippingRequestVasId",
                table: "ShippingRequestTripVases",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ShippingRequestId",
                table: "ShippingRequestDestinationCities",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ShippingTypeId",
                table: "PricePackages",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<byte>(
                name: "RouteType",
                table: "PricePackages",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PricePackages_ShippingTypeId",
                table: "PricePackages",
                column: "ShippingTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_PricePackages_ShippingTypes_ShippingTypeId",
                table: "PricePackages",
                column: "ShippingTypeId",
                principalTable: "ShippingTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestDestinationCities_ShippingRequests_ShippingRequestId",
                table: "ShippingRequestDestinationCities",
                column: "ShippingRequestId",
                principalTable: "ShippingRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTripVases_ShippingRequestVases_ShippingRequestVasId",
                table: "ShippingRequestTripVases",
                column: "ShippingRequestVasId",
                principalTable: "ShippingRequestVases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
