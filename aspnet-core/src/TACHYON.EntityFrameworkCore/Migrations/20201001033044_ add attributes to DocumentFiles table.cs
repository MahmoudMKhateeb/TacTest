using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addattributestoDocumentFilestable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasNotes",
                table: "DocumentFiles",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasNumber",
                table: "DocumentFiles",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "DocumentFiles",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "DocumentFiles",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasNotes",
                table: "DocumentFiles");

            migrationBuilder.DropColumn(
                name: "HasNumber",
                table: "DocumentFiles");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "DocumentFiles");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "DocumentFiles");
        }
    }
}
