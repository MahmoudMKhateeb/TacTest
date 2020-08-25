using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class updateSourceFacilityIdDestinationFacilityIdtonullableinRoutStepstable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_Facilities_DestinationFacilityId",
                table: "RoutSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_Facilities_SourceFacilityId",
                table: "RoutSteps");

            migrationBuilder.AlterColumn<long>(
                name: "SourceFacilityId",
                table: "RoutSteps",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "DestinationFacilityId",
                table: "RoutSteps",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

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

            migrationBuilder.AlterColumn<long>(
                name: "SourceFacilityId",
                table: "RoutSteps",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "DestinationFacilityId",
                table: "RoutSteps",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_Facilities_DestinationFacilityId",
                table: "RoutSteps",
                column: "DestinationFacilityId",
                principalTable: "Facilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_Facilities_SourceFacilityId",
                table: "RoutSteps",
                column: "SourceFacilityId",
                principalTable: "Facilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
