using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class updateGoodCatgoriestable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BayanIntegrationId",
                table: "GoodCategories",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "GoodCategories",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BayanIntegrationId",
                table: "GoodCategories");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "GoodCategories");
        }
    }
}
