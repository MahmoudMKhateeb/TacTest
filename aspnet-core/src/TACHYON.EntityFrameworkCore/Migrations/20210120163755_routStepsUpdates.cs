using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class routStepsUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_Trailers_AssignedTrailerId",
                table: "RoutSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_TrailerTypes_TrailerTypeId",
                table: "RoutSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_TrucksTypes_TrucksTypeId",
                table: "RoutSteps");

            migrationBuilder.DropIndex(
                name: "IX_RoutSteps_AssignedTrailerId",
                table: "RoutSteps");

            migrationBuilder.DropIndex(
                name: "IX_RoutSteps_TrailerTypeId",
                table: "RoutSteps");

            migrationBuilder.DropIndex(
                name: "IX_RoutSteps_TrucksTypeId",
                table: "RoutSteps");

            migrationBuilder.DropColumn(
                name: "AssignedTrailerId",
                table: "RoutSteps");

            migrationBuilder.DropColumn(
                name: "TrailerTypeId",
                table: "RoutSteps");

            migrationBuilder.DropColumn(
                name: "TrucksTypeId",
                table: "RoutSteps");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Facilities");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfTrips",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTripDate",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "GoodsDetails",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfTrips",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "StartTripDate",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "GoodsDetails");

            migrationBuilder.AddColumn<long>(
                name: "AssignedTrailerId",
                table: "RoutSteps",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrailerTypeId",
                table: "RoutSteps",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TrucksTypeId",
                table: "RoutSteps",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Latitude",
                table: "RoutPoints",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Longitude",
                table: "RoutPoints",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Ports",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "Ports",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Facilities",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "Facilities",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_RoutSteps_AssignedTrailerId",
                table: "RoutSteps",
                column: "AssignedTrailerId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutSteps_TrailerTypeId",
                table: "RoutSteps",
                column: "TrailerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutSteps_TrucksTypeId",
                table: "RoutSteps",
                column: "TrucksTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_Trailers_AssignedTrailerId",
                table: "RoutSteps",
                column: "AssignedTrailerId",
                principalTable: "Trailers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_TrailerTypes_TrailerTypeId",
                table: "RoutSteps",
                column: "TrailerTypeId",
                principalTable: "TrailerTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_TrucksTypes_TrucksTypeId",
                table: "RoutSteps",
                column: "TrucksTypeId",
                principalTable: "TrucksTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
