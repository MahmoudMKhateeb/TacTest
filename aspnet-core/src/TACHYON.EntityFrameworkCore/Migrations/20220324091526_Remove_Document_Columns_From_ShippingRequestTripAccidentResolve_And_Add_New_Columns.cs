using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Remove_Document_Columns_From_ShippingRequestTripAccidentResolve_And_Add_New_Columns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "ShippingRequestTripAccidentResolves");

            migrationBuilder.DropColumn(
                name: "DocumentContentType",
                table: "ShippingRequestTripAccidentResolves");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "ShippingRequestTripAccidentResolves");

            migrationBuilder.DropColumn(
                name: "DocumentName",
                table: "ShippingRequestTripAccidentResolves");

            migrationBuilder.AddColumn<bool>(
                name: "ApprovedByCarrier",
                table: "ShippingRequestTripAccidentResolves",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ApprovedByShipper",
                table: "ShippingRequestTripAccidentResolves",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "DriverId",
                table: "ShippingRequestTripAccidentResolves",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApplied",
                table: "ShippingRequestTripAccidentResolves",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ResolveType",
                table: "ShippingRequestTripAccidentResolves",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "TruckId",
                table: "ShippingRequestTripAccidentResolves",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedByCarrier",
                table: "ShippingRequestTripAccidentResolves");

            migrationBuilder.DropColumn(
                name: "ApprovedByShipper",
                table: "ShippingRequestTripAccidentResolves");

            migrationBuilder.DropColumn(
                name: "DriverId",
                table: "ShippingRequestTripAccidentResolves");

            migrationBuilder.DropColumn(
                name: "IsApplied",
                table: "ShippingRequestTripAccidentResolves");

            migrationBuilder.DropColumn(
                name: "ResolveType",
                table: "ShippingRequestTripAccidentResolves");

            migrationBuilder.DropColumn(
                name: "TruckId",
                table: "ShippingRequestTripAccidentResolves");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ShippingRequestTripAccidentResolves",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentContentType",
                table: "ShippingRequestTripAccidentResolves",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DocumentId",
                table: "ShippingRequestTripAccidentResolves",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentName",
                table: "ShippingRequestTripAccidentResolves",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
