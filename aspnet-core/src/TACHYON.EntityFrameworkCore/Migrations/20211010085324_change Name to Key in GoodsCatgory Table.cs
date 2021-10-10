using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class changeNametoKeyinGoodsCatgoryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "GoodCategories");

            //migrationBuilder.AddColumn<string>(
            //    name: "DisplayName",
            //    table: "TrucksTypes",
            //    maxLength: 256,
            //    nullable: false,
            //    defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "ShippingRequestReasonAccidents",
                maxLength: 60,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "GoodCategories",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "DisplayName",
            //    table: "TrucksTypes");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "ShippingRequestReasonAccidents");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "GoodCategories");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "GoodCategories",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }
    }
}
