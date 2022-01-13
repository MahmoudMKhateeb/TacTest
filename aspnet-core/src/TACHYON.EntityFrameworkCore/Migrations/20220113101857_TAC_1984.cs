using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class TAC_1984 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReset",
                table: "RoutPointStatusTransitions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanGoToNextLocation",
                table: "RoutPoints",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPodUploaded",
                table: "RoutPoints",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsResolve",
                table: "RoutPoints",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "WorkFlowVersion",
                table: "RoutPoints",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReset",
                table: "RoutPointStatusTransitions");

            migrationBuilder.DropColumn(
                name: "CanGoToNextLocation",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "IsPodUploaded",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "IsResolve",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "WorkFlowVersion",
                table: "RoutPoints");
        }
    }
}
