using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class tenantmissingregisterationdetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "AbpTenants",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "AbpTenants",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserTitle",
                table: "AbpTenants",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "UserTitle",
                table: "AbpTenants");
        }
    }
}
