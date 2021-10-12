using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using System;

namespace TACHYON.Migrations
{
    public partial class DriverLocationLog_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<string>(
            //    name: "ShipperInvoiceNo",
            //    table: "ShippingRequests",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "ShipperReference",
            //    table: "ShippingRequests",
            //    nullable: true);

            migrationBuilder.CreateTable(
                name: "DriverLocationLogs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatorUserId = table.Column<long>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    TripId = table.Column<int>(nullable: true),
                    Location = table.Column<Point>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverLocationLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DriverLocationLogs_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DriverLocationLogs_ShippingRequestTrips_TripId",
                        column: x => x.TripId,
                        principalTable: "ShippingRequestTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DriverLocationLogs_CreatorUserId",
                table: "DriverLocationLogs",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DriverLocationLogs_TripId",
                table: "DriverLocationLogs",
                column: "TripId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DriverLocationLogs");

            //migrationBuilder.DropColumn(
            //    name: "ShipperInvoiceNo",
            //    table: "ShippingRequests");

            //migrationBuilder.DropColumn(
            //    name: "ShipperReference",
            //    table: "ShippingRequests");
        }
    }
}