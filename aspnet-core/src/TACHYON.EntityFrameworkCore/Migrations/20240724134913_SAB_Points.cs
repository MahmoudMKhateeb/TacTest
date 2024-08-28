using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class SAB_Points : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DriverUserId",
                table: "RoutPoints",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StorageDays",
                table: "RoutPoints",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "StoragePricePerDay",
                table: "RoutPoints",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TruckId",
                table: "RoutPoints",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriverUserId",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "StorageDays",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "StoragePricePerDay",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "TruckId",
                table: "RoutPoints");
        }
    }
}
