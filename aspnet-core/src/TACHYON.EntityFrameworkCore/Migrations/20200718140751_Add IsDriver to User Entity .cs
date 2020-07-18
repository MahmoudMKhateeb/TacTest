using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddIsDrivertoUserEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Phone",
                table: "AbpUsers");

            migrationBuilder.AddColumn<bool>(
                name: "IsDriver",
                table: "AbpUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDriver",
                table: "AbpUsers");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
