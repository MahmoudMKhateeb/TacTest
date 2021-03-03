using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class RoutePointTranstion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "RoutPoints",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentId",
                table: "RoutPoints",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RoutePointTranstions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromPointId = table.Column<long>(nullable: false),
                    ToPointId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutePointTranstions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoutePointTranstions_RoutPoints_FromPointId",
                        column: x => x.FromPointId,
                        principalTable: "RoutPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoutePointTranstions_RoutPoints_ToPointId",
                        column: x => x.ToPointId,
                        principalTable: "RoutPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShippingRequestTripsPoints",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TripId = table.Column<int>(nullable: false),
                    PointId = table.Column<long>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: true),
                    EndTime = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsComplete = table.Column<bool>(nullable: false),
                    DocumentId = table.Column<Guid>(nullable: true),
                    DocumentName = table.Column<string>(nullable: true),
                    DocumentContentType = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Rating = table.Column<int>(nullable: true)
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
                name: "IX_RoutPoints_ParentId",
                table: "RoutPoints",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutePointTranstions_FromPointId",
                table: "RoutePointTranstions",
                column: "FromPointId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutePointTranstions_ToPointId",
                table: "RoutePointTranstions",
                column: "ToPointId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTripsPoints_TripId",
                table: "ShippingRequestTripsPoints",
                column: "TripId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoutPoints_RoutPoints_ParentId",
                table: "RoutPoints",
                column: "ParentId",
                principalTable: "RoutPoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoutPoints_RoutPoints_ParentId",
                table: "RoutPoints");

            migrationBuilder.DropTable(
                name: "RoutePointTranstions");

            migrationBuilder.DropTable(
                name: "ShippingRequestTripsPoints");

            migrationBuilder.DropIndex(
                name: "IX_RoutPoints_ParentId",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "RoutPoints");
        }
    }
}
