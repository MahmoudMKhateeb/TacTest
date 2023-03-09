using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class port_movements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_ShippingTypes_ShippingTypeId",
                table: "ShippingRequests");

            //migrationBuilder.DropIndex(
            //    name: "IX_ShippingRequests_ShippingTypeId",
            //    table: "ShippingRequests");

            migrationBuilder.AddColumn<long>(
                name: "RoutePointId",
                table: "ShippingRequestTripVases",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OriginFacilityId",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "RoundTripType",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AdditionalStepWorkFlowVersion",
                table: "RoutPoints",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AppointmentDateTime",
                table: "RoutPoints",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AppointmentNumber",
                table: "RoutPoints",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasAppointmentVas",
                table: "RoutPoints",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasClearanceVas",
                table: "RoutPoints",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NeedsAppointment",
                table: "RoutPoints",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NeedsClearance",
                table: "RoutPoints",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PointOrder",
                table: "RoutPoints",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Flag",
                table: "GoodCategories",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FacilityType",
                table: "Facilities",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AdditionalStepTransitions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    RoutePointId = table.Column<long>(nullable: false),
                    AdditionalStepType = table.Column<int>(nullable: false),
                    RoutePointDocumentType = table.Column<byte>(nullable: true),
                    IsFile = table.Column<bool>(nullable: false),
                    IsReset = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdditionalStepTransitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdditionalStepTransitions_RoutPoints_RoutePointId",
                        column: x => x.RoutePointId,
                        principalTable: "RoutPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTripVases_RoutePointId",
                table: "ShippingRequestTripVases",
                column: "RoutePointId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_OriginFacilityId",
                table: "ShippingRequests",
                column: "OriginFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_AdditionalStepTransitions_RoutePointId",
                table: "AdditionalStepTransitions",
                column: "RoutePointId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_Facilities_OriginFacilityId",
                table: "ShippingRequests",
                column: "OriginFacilityId",
                principalTable: "Facilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTripVases_RoutPoints_RoutePointId",
                table: "ShippingRequestTripVases",
                column: "RoutePointId",
                principalTable: "RoutPoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_Facilities_OriginFacilityId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTripVases_RoutPoints_RoutePointId",
                table: "ShippingRequestTripVases");

            migrationBuilder.DropTable(
                name: "AdditionalStepTransitions");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTripVases_RoutePointId",
                table: "ShippingRequestTripVases");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_OriginFacilityId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "RoutePointId",
                table: "ShippingRequestTripVases");

            migrationBuilder.DropColumn(
                name: "OriginFacilityId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "RoundTripType",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "AdditionalStepWorkFlowVersion",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "AppointmentDateTime",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "AppointmentNumber",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "HasAppointmentVas",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "HasClearanceVas",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "NeedsAppointment",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "NeedsClearance",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "PointOrder",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "Flag",
                table: "GoodCategories");

            migrationBuilder.DropColumn(
                name: "FacilityType",
                table: "Facilities");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_ShippingTypeId",
                table: "ShippingRequests",
                column: "ShippingTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_ShippingTypes_ShippingTypeId",
                table: "ShippingRequests",
                column: "ShippingTypeId",
                principalTable: "ShippingTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
