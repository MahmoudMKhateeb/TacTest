using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class release_2_3_0 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DriverUserId",
                table: "Trucks",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfDrops",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "RouteType",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ExpectedMileage",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfTrucks",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RentalDuration",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "RentalDurationUnit",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RentalEndDate",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RentalStartDate",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceAreaNotes",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "ShippingRequestFlag",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<Guid>(
                name: "ProposalFileId",
                table: "PricePackageProposals",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DedicatedShippingRequestDrivers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    ShippingRequestId = table.Column<long>(nullable: false),
                    DriverUserId = table.Column<long>(nullable: false),
                    Status = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DedicatedShippingRequestDrivers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DedicatedShippingRequestDrivers_AbpUsers_DriverUserId",
                        column: x => x.DriverUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DedicatedShippingRequestDrivers_ShippingRequests_ShippingRequestId",
                        column: x => x.ShippingRequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DedicatedShippingRequestTrucks",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    ShippingRequestId = table.Column<long>(nullable: false),
                    TruckId = table.Column<long>(nullable: false),
                    Status = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DedicatedShippingRequestTrucks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DedicatedShippingRequestTrucks_ShippingRequests_ShippingRequestId",
                        column: x => x.ShippingRequestId,
                        principalTable: "ShippingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DedicatedShippingRequestTrucks_Trucks_TruckId",
                        column: x => x.TruckId,
                        principalTable: "Trucks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_DriverUserId",
                table: "Trucks",
                column: "DriverUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedShippingRequestDrivers_DriverUserId",
                table: "DedicatedShippingRequestDrivers",
                column: "DriverUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedShippingRequestDrivers_ShippingRequestId",
                table: "DedicatedShippingRequestDrivers",
                column: "ShippingRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedShippingRequestTrucks_ShippingRequestId",
                table: "DedicatedShippingRequestTrucks",
                column: "ShippingRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_DedicatedShippingRequestTrucks_TruckId",
                table: "DedicatedShippingRequestTrucks",
                column: "TruckId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trucks_AbpUsers_DriverUserId",
                table: "Trucks",
                column: "DriverUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trucks_AbpUsers_DriverUserId",
                table: "Trucks");

            migrationBuilder.DropTable(
                name: "DedicatedShippingRequestDrivers");

            migrationBuilder.DropTable(
                name: "DedicatedShippingRequestTrucks");

            migrationBuilder.DropIndex(
                name: "IX_Trucks_DriverUserId",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "DriverUserId",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "NumberOfDrops",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "RouteType",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "ExpectedMileage",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "NumberOfTrucks",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "RentalDuration",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "RentalDurationUnit",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "RentalEndDate",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "RentalStartDate",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "ServiceAreaNotes",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "ShippingRequestFlag",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "ProposalFileId",
                table: "PricePackageProposals");
        }
    }
}
