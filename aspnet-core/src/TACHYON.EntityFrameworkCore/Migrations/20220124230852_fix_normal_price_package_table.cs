using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class fix_normal_price_package_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "NormalPricePackages");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "NormalPricePackages",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PricePackageId",
                table: "NormalPricePackages",
                nullable: true);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "NormalPricePackages");

            migrationBuilder.DropColumn(
                name: "PricePackageId",
                table: "NormalPricePackages");


            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "NormalPricePackages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}