using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Add_driverLicenseTypeIdTo_User : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DriverLicenseTypeId",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_DriverLicenseTypeId",
                table: "AbpUsers",
                column: "DriverLicenseTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpUsers_DriverLicenseTypes_DriverLicenseTypeId",
                table: "AbpUsers",
                column: "DriverLicenseTypeId",
                principalTable: "DriverLicenseTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpUsers_DriverLicenseTypes_DriverLicenseTypeId",
                table: "AbpUsers");

            migrationBuilder.DropIndex(
                name: "IX_AbpUsers_DriverLicenseTypeId",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "DriverLicenseTypeId",
                table: "AbpUsers");
        }
    }
}