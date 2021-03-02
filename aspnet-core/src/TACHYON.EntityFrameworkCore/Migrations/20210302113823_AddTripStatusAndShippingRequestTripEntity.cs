using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddTripStatusAndShippingRequestTripEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TripStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    DisplayName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShippingRequestTrips",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    StartTripDate = table.Column<DateTime>(nullable: false),
                    EndTripDate = table.Column<DateTime>(nullable: false),
                    TripStatusId = table.Column<int>(nullable: false),
                    AssignedDriverUserId = table.Column<long>(nullable: true),
                    AssignedTruckId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestTrips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRequestTrips_AbpUsers_AssignedDriverUserId",
                        column: x => x.AssignedDriverUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingRequestTrips_Trucks_AssignedTruckId",
                        column: x => x.AssignedTruckId,
                        principalTable: "Trucks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingRequestTrips_TripStatuses_TripStatusId",
                        column: x => x.TripStatusId,
                        principalTable: "TripStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_AssignedDriverUserId",
                table: "ShippingRequestTrips",
                column: "AssignedDriverUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_AssignedTruckId",
                table: "ShippingRequestTrips",
                column: "AssignedTruckId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_TripStatusId",
                table: "ShippingRequestTrips",
                column: "TripStatusId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShippingRequestTrips");

            migrationBuilder.DropTable(
                name: "TripStatuses");
        }
    }
}
