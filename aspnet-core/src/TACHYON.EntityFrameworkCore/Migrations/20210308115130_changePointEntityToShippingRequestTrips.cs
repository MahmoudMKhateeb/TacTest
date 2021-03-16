using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class changePointEntityToShippingRequestTrips : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_GoodsDetails_ShippingRequests_ShippingRequestId",
            //    table: "GoodsDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Facilities_DestinationFacilityId",
                table: "Routes");

            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Facilities_OriginFacilityId",
                table: "Routes");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutPoints_ShippingRequests_ShippingRequestId",
                table: "RoutPoints");

            migrationBuilder.DropTable(
                name: "ShippingRequestTripsPoints");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestVases_TenantId",
                table: "ShippingRequestVases");

            migrationBuilder.DropIndex(
                name: "IX_RoutPoints_ShippingRequestId",
                table: "RoutPoints");

            migrationBuilder.DropIndex(
                name: "IX_Routes_DestinationFacilityId",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Routes_OriginFacilityId",
                table: "Routes");

            //migrationBuilder.DropIndex(
            //    name: "IX_GoodsDetails_ShippingRequestId",
            //    table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ShippingRequestVases");

            migrationBuilder.DropColumn(
                name: "ShippingRequestId",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "DestinationFacilityId",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "OriginFacilityId",
                table: "Routes");

            //migrationBuilder.DropColumn(
            //    name: "ShippingRequestId",
            //    table: "GoodsDetails");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfTrips",
                table: "ShippingRequestVases",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "DestinationFacilityId",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OriginFacilityId",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentContentType",
                table: "RoutPoints",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DocumentId",
                table: "RoutPoints",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentName",
                table: "RoutPoints",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "RoutPoints",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "RoutPoints",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsComplete",
                table: "RoutPoints",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "RoutPoints",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShippingRequestTripId",
                table: "RoutPoints",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "RoutPoints",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_DestinationFacilityId",
                table: "ShippingRequestTrips",
                column: "DestinationFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_OriginFacilityId",
                table: "ShippingRequestTrips",
                column: "OriginFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutPoints_ShippingRequestTripId",
                table: "RoutPoints",
                column: "ShippingRequestTripId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoutPoints_ShippingRequestTrips_ShippingRequestTripId",
                table: "RoutPoints",
                column: "ShippingRequestTripId",
                principalTable: "ShippingRequestTrips",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTrips_Facilities_DestinationFacilityId",
                table: "ShippingRequestTrips",
                column: "DestinationFacilityId",
                principalTable: "Facilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTrips_Facilities_OriginFacilityId",
                table: "ShippingRequestTrips",
                column: "OriginFacilityId",
                principalTable: "Facilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoutPoints_ShippingRequestTrips_ShippingRequestTripId",
                table: "RoutPoints");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTrips_Facilities_DestinationFacilityId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTrips_Facilities_OriginFacilityId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTrips_DestinationFacilityId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTrips_OriginFacilityId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropIndex(
                name: "IX_RoutPoints_ShippingRequestTripId",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "NumberOfTrips",
                table: "ShippingRequestVases");

            migrationBuilder.DropColumn(
                name: "DestinationFacilityId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "OriginFacilityId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "DocumentContentType",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "DocumentName",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "IsComplete",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "ShippingRequestTripId",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "RoutPoints");

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "ShippingRequestVases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "ShippingRequestId",
                table: "RoutPoints",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "DestinationFacilityId",
                table: "Routes",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OriginFacilityId",
                table: "Routes",
                type: "bigint",
                nullable: true);

            //migrationBuilder.AddColumn<long>(
            //    name: "ShippingRequestId",
            //    table: "GoodsDetails",
            //    type: "bigint",
            //    nullable: true);

            migrationBuilder.CreateTable(
                name: "ShippingRequestTripsPoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocumentContentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DocumentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsComplete = table.Column<bool>(type: "bit", nullable: false),
                    PointId = table.Column<long>(type: "bigint", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TripId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestTripsPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRequestTripsPoints_ShippingRequestTrips_TripId",
                        column: x => x.TripId,
                        principalTable: "ShippingRequestTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestVases_TenantId",
                table: "ShippingRequestVases",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutPoints_ShippingRequestId",
                table: "RoutPoints",
                column: "ShippingRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_DestinationFacilityId",
                table: "Routes",
                column: "DestinationFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_OriginFacilityId",
                table: "Routes",
                column: "OriginFacilityId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_GoodsDetails_ShippingRequestId",
            //    table: "GoodsDetails",
            //    column: "ShippingRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTripsPoints_TripId",
                table: "ShippingRequestTripsPoints",
                column: "TripId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_GoodsDetails_ShippingRequests_ShippingRequestId",
            //    table: "GoodsDetails",
            //    column: "ShippingRequestId",
            //    principalTable: "ShippingRequests",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Facilities_DestinationFacilityId",
                table: "Routes",
                column: "DestinationFacilityId",
                principalTable: "Facilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Facilities_OriginFacilityId",
                table: "Routes",
                column: "OriginFacilityId",
                principalTable: "Facilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutPoints_ShippingRequests_ShippingRequestId",
                table: "RoutPoints",
                column: "ShippingRequestId",
                principalTable: "ShippingRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
