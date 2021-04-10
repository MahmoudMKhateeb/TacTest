using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class TACHYONDB_Devlopment_abdullah : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShippingRequestCausesAccidents");

            migrationBuilder.DropTable(
                name: "TripStatuses");

            migrationBuilder.AddColumn<bool>(
                name: "HasAccident",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasAccident",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproveCancledByCarrier",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproveCancledByShipper",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ShippingRequestReasonAccidents",
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
                    DisplayName = table.Column<string>(maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestReasonAccidents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShippingRequestTripAccidents",
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
                    PointId = table.Column<long>(nullable: false),
                    ResoneId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    IsResolve = table.Column<bool>(nullable: false),
                    DocumentId = table.Column<Guid>(nullable: true),
                    DocumentName = table.Column<string>(nullable: true),
                    DocumentContentType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestTripAccidents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRequestTripAccidents_RoutPoints_PointId",
                        column: x => x.PointId,
                        principalTable: "RoutPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShippingRequestTripAccidents_ShippingRequestReasonAccidents_ResoneId",
                        column: x => x.ResoneId,
                        principalTable: "ShippingRequestReasonAccidents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTripAccidents_PointId",
                table: "ShippingRequestTripAccidents",
                column: "PointId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTripAccidents_ResoneId",
                table: "ShippingRequestTripAccidents",
                column: "ResoneId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShippingRequestTripAccidents");

            migrationBuilder.DropTable(
                name: "ShippingRequestReasonAccidents");

            migrationBuilder.DropColumn(
                name: "HasAccident",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "HasAccident",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "IsApproveCancledByCarrier",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "IsApproveCancledByShipper",
                table: "ShippingRequests");

            migrationBuilder.CreateTable(
                name: "ShippingRequestCausesAccidents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRequestCausesAccidents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TripStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripStatuses", x => x.Id);
                });
        }
    }
}
