using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addSourceFacilityIdDestinationFacilityIdtoRoutStepstable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DestinationFacilityId",
                table: "RoutSteps",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "SourceFacilityId",
                table: "RoutSteps",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_RoutSteps_DestinationFacilityId",
                table: "RoutSteps",
                column: "DestinationFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutSteps_SourceFacilityId",
                table: "RoutSteps",
                column: "SourceFacilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_Facilities_DestinationFacilityId",
                table: "RoutSteps",
                column: "DestinationFacilityId",
                principalTable: "Facilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_Facilities_SourceFacilityId",
                table: "RoutSteps",
                column: "SourceFacilityId",
                principalTable: "Facilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_Facilities_DestinationFacilityId",
                table: "RoutSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_Facilities_SourceFacilityId",
                table: "RoutSteps");

            migrationBuilder.DropIndex(
                name: "IX_RoutSteps_DestinationFacilityId",
                table: "RoutSteps");

            migrationBuilder.DropIndex(
                name: "IX_RoutSteps_SourceFacilityId",
                table: "RoutSteps");

            migrationBuilder.DropColumn(
                name: "DestinationFacilityId",
                table: "RoutSteps");

            migrationBuilder.DropColumn(
                name: "SourceFacilityId",
                table: "RoutSteps");
        }
    }
}
