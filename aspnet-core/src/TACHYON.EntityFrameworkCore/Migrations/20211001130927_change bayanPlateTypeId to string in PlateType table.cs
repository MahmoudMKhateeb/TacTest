using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class changebayanPlateTypeIdtostringinPlateTypetable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BayanPlatetypeId",
                table: "PlateTypes");

            migrationBuilder.AddColumn<string>(
                name: "BayanIntegrationId",
                table: "PlateTypes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "PlateTypes",
                maxLength: 64,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BayanIntegrationId",
                table: "PlateTypes");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "PlateTypes");

            migrationBuilder.AddColumn<int>(
                name: "BayanPlatetypeId",
                table: "PlateTypes",
                type: "int",
                nullable: true);
        }
    }
}
