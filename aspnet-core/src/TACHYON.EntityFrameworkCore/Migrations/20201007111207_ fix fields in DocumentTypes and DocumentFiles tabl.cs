using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class fixfieldsinDocumentTypesandDocumentFilestabl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasSpecialConstant",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "SpecialConstant",
                table: "DocumentFiles");

            migrationBuilder.AddColumn<string>(
                name: "SpecialConstant",
                table: "DocumentTypes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HijriExpirationDate",
                table: "DocumentFiles",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpecialConstant",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "HijriExpirationDate",
                table: "DocumentFiles");

            migrationBuilder.AddColumn<bool>(
                name: "HasSpecialConstant",
                table: "DocumentTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SpecialConstant",
                table: "DocumentFiles",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
