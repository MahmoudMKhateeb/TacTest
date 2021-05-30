using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class FormatNumbers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "WaybillNumber",
                table: "RoutPoints",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountNumber",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoutPoints_WaybillNumber",
                table: "RoutPoints",
                column: "WaybillNumber",
                unique: true,
                filter: "[WaybillNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_AccountNumber",
                table: "AbpUsers",
                column: "AccountNumber",
                unique: true,
                filter: "[AccountNumber] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RoutPoints_WaybillNumber",
                table: "RoutPoints");

            migrationBuilder.DropIndex(
                name: "IX_AbpUsers_AccountNumber",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "WaybillNumber",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "AccountNumber",
                table: "AbpUsers");
        }
    }
}
