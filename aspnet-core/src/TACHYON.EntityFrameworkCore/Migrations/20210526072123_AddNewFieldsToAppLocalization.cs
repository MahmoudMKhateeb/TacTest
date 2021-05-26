using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddNewFieldsToAppLocalization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppVersion",
                table: "AppLocalizations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "PlatForm",
                table: "AppLocalizations",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "Version",
                table: "AppLocalizations",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppVersion",
                table: "AppLocalizations");

            migrationBuilder.DropColumn(
                name: "PlatForm",
                table: "AppLocalizations");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "AppLocalizations");
        }
    }
}
