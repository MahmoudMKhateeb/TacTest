using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddIsActiveToTruckTypeEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TrucksTypes",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "GoodCategories",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TrucksTypes");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "GoodCategories",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldDefaultValue: true);
        }
    }
}
