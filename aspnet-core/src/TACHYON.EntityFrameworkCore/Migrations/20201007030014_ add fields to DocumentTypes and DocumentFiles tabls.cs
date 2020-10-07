using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addfieldstoDocumentTypesandDocumentFilestabls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExpirationAlertDays",
                table: "DocumentTypes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "HasSpecialConstant",
                table: "DocumentTypes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InActiveAccountExpired",
                table: "DocumentTypes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "InActiveToleranceDays",
                table: "DocumentTypes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberMaxDigits",
                table: "DocumentTypes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberMinDigits",
                table: "DocumentTypes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SpecialConstant",
                table: "DocumentFiles",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpirationAlertDays",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "HasSpecialConstant",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "InActiveAccountExpired",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "InActiveToleranceDays",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "NumberMaxDigits",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "NumberMinDigits",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "SpecialConstant",
                table: "DocumentFiles");
        }
    }
}
