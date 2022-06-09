using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class RemoveRequiredFacility_FacilityWorkingHourTB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FacilityWorkingHour_Facilities_FacilityId",
                table: "FacilityWorkingHour");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FacilityWorkingHour",
                table: "FacilityWorkingHour");

            migrationBuilder.RenameTable(
                name: "FacilityWorkingHour",
                newName: "FacilityWorkingHours");

            migrationBuilder.RenameIndex(
                name: "IX_FacilityWorkingHour_FacilityId",
                table: "FacilityWorkingHours",
                newName: "IX_FacilityWorkingHours_FacilityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FacilityWorkingHours",
                table: "FacilityWorkingHours",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FacilityWorkingHours_Facilities_FacilityId",
                table: "FacilityWorkingHours",
                column: "FacilityId",
                principalTable: "Facilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FacilityWorkingHours_Facilities_FacilityId",
                table: "FacilityWorkingHours");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FacilityWorkingHours",
                table: "FacilityWorkingHours");

            migrationBuilder.RenameTable(
                name: "FacilityWorkingHours",
                newName: "FacilityWorkingHour");

            migrationBuilder.RenameIndex(
                name: "IX_FacilityWorkingHours_FacilityId",
                table: "FacilityWorkingHour",
                newName: "IX_FacilityWorkingHour_FacilityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FacilityWorkingHour",
                table: "FacilityWorkingHour",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FacilityWorkingHour_Facilities_FacilityId",
                table: "FacilityWorkingHour",
                column: "FacilityId",
                principalTable: "Facilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
