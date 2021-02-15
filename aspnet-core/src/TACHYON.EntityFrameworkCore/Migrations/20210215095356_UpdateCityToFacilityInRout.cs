using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class UpdateCityToFacilityInRout : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Cities_DestinationCityId",
                table: "Routes");

            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Cities_OriginCityId",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Routes_DestinationCityId",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Routes_OriginCityId",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "DestinationCityId",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "OriginCityId",
                table: "Routes");

            migrationBuilder.AddColumn<long>(
                name: "DestinationFacilityId",
                table: "Routes",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OriginFacilityId",
                table: "Routes",
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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "DestinationCityId",
                table: "Routes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OriginCityId",
                table: "Routes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Routes_DestinationCityId",
                table: "Routes",
                column: "DestinationCityId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_OriginCityId",
                table: "Routes",
                column: "OriginCityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Cities_DestinationCityId",
                table: "Routes",
                column: "DestinationCityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Cities_OriginCityId",
                table: "Routes",
                column: "OriginCityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
