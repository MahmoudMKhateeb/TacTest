using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class rating : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Rate",
                table: "AbpUsers",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Rate",
                table: "AbpTenants",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "RatingLogs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceiverId = table.Column<int>(nullable: true),
                    DriverId = table.Column<long>(nullable: true),
                    ShipperId = table.Column<int>(nullable: true),
                    CarrierId = table.Column<int>(nullable: true),
                    PointId = table.Column<long>(nullable: true),
                    TripId = table.Column<int>(nullable: true),
                    FacilityId = table.Column<long>(nullable: true),
                    RateType = table.Column<byte>(nullable: false),
                    Rate = table.Column<int>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    Code = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatingLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RatingLogs_AbpTenants_CarrierId",
                        column: x => x.CarrierId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RatingLogs_AbpUsers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RatingLogs_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RatingLogs_RoutPoints_PointId",
                        column: x => x.PointId,
                        principalTable: "RoutPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RatingLogs_Receivers_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Receivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RatingLogs_AbpTenants_ShipperId",
                        column: x => x.ShipperId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RatingLogs_ShippingRequestTrips_TripId",
                        column: x => x.TripId,
                        principalTable: "ShippingRequestTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RatingLogs_CarrierId",
                table: "RatingLogs",
                column: "CarrierId");

            migrationBuilder.CreateIndex(
                name: "IX_RatingLogs_DriverId",
                table: "RatingLogs",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_RatingLogs_FacilityId",
                table: "RatingLogs",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_RatingLogs_PointId",
                table: "RatingLogs",
                column: "PointId");

            migrationBuilder.CreateIndex(
                name: "IX_RatingLogs_ReceiverId",
                table: "RatingLogs",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_RatingLogs_ShipperId",
                table: "RatingLogs",
                column: "ShipperId");

            migrationBuilder.CreateIndex(
                name: "IX_RatingLogs_TripId",
                table: "RatingLogs",
                column: "TripId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RatingLogs");

            migrationBuilder.DropColumn(
                name: "Rate",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "Rate",
                table: "AbpTenants");
        }
    }
}
