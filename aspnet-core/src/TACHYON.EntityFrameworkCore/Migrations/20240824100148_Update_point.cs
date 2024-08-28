using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Update_point : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Distance",
                table: "RoutPoints",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DriverWorkingHour",
                table: "RoutPoints",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Distance",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "DriverWorkingHour",
                table: "RoutPoints");
        }
    }
}
