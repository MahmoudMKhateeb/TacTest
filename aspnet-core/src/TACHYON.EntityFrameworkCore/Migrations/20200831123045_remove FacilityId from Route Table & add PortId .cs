using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class removeFacilityIdfromRouteTableaddPortId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Facilities_DestinationFacilityId",
                table: "Routes");

            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Facilities_OriginFacilityId",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Routes_DestinationFacilityId",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Routes_OriginFacilityId",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "DestinationFacilityId",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "OriginFacilityId",
                table: "Routes");

            migrationBuilder.AddColumn<long>(
                name: "DestinationPortId",
                table: "Routes",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OriginPortId",
                table: "Routes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Routes_DestinationPortId",
                table: "Routes",
                column: "DestinationPortId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_OriginPortId",
                table: "Routes",
                column: "OriginPortId");

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Ports_DestinationPortId",
                table: "Routes",
                column: "DestinationPortId",
                principalTable: "Ports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Ports_OriginPortId",
                table: "Routes",
                column: "OriginPortId",
                principalTable: "Ports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Ports_DestinationPortId",
                table: "Routes");

            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Ports_OriginPortId",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Routes_DestinationPortId",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Routes_OriginPortId",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "DestinationPortId",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "OriginPortId",
                table: "Routes");

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

            migrationBuilder.CreateIndex(
                name: "IX_Routes_DestinationFacilityId",
                table: "Routes",
                column: "DestinationFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_OriginFacilityId",
                table: "Routes",
                column: "OriginFacilityId");

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
        }
    }
}
