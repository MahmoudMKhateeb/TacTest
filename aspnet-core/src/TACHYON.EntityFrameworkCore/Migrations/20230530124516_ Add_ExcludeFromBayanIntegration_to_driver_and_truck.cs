using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Add_ExcludeFromBayanIntegration_to_driver_and_truck : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ExcludeFromBayanIntegration",
                table: "Trucks",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ExcludeFromBayanIntegration",
                table: "AbpUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExcludeFromBayanIntegration",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "ExcludeFromBayanIntegration",
                table: "AbpUsers");
        }
    }
}
