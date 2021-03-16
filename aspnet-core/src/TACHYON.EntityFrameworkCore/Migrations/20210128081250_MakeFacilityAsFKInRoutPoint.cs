using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class MakeFacilityAsFKInRoutPoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoutPoints_Facilities_SourceFacilityId",
                table: "RoutPoints");

            migrationBuilder.DropIndex(
                name: "IX_RoutPoints_SourceFacilityId",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "SourceFacilityId",
                table: "RoutPoints");

            migrationBuilder.CreateIndex(
                name: "IX_RoutPoints_FacilityId",
                table: "RoutPoints",
                column: "FacilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoutPoints_Facilities_FacilityId",
                table: "RoutPoints",
                column: "FacilityId",
                principalTable: "Facilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoutPoints_Facilities_FacilityId",
                table: "RoutPoints");

            migrationBuilder.DropIndex(
                name: "IX_RoutPoints_FacilityId",
                table: "RoutPoints");

            migrationBuilder.AddColumn<long>(
                name: "SourceFacilityId",
                table: "RoutPoints",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoutPoints_SourceFacilityId",
                table: "RoutPoints",
                column: "SourceFacilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoutPoints_Facilities_SourceFacilityId",
                table: "RoutPoints",
                column: "SourceFacilityId",
                principalTable: "Facilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
