using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class _11_10_2021 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "BayanPlatetypeId",
            //    table: "PlateTypes");

            //migrationBuilder.AddColumn<string>(
            //    name: "BayanIntegrationId",
            //    table: "PlateTypes",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "Name",
            //    table: "PlateTypes",
            //    maxLength: 64,
            //    nullable: false,
            //    defaultValue: "");

            //migrationBuilder.AddColumn<string>(
            //    name: "BayanIntegrationId",
            //    table: "GoodCategories",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "Name",
            //    table: "GoodCategories",
            //    maxLength: 256,
            //    nullable: false,
            //    defaultValue: "");

            //migrationBuilder.AddColumn<string>(
            //    name: "MoiNumber",
            //    table: "AbpTenants",
            //    nullable: false,
            //    defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "BayanIntegrationId",
            //    table: "PlateTypes");

            //migrationBuilder.DropColumn(
            //    name: "Name",
            //    table: "PlateTypes");

            //migrationBuilder.DropColumn(
            //    name: "BayanIntegrationId",
            //    table: "GoodCategories");

            //migrationBuilder.DropColumn(
            //    name: "Name",
            //    table: "GoodCategories");

            //migrationBuilder.DropColumn(
            //    name: "MoiNumber",
            //    table: "AbpTenants");

            //migrationBuilder.AddColumn<int>(
            //    name: "BayanPlatetypeId",
            //    table: "PlateTypes",
            //    type: "int",
            //    nullable: true);
        }
    }
}