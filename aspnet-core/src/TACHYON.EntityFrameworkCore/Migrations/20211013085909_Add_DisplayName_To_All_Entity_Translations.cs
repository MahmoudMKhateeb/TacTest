using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Add_DisplayName_To_All_Entity_Translations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "TrucksTypesTranslations",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "TransportTypesTranslations",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "ShippingRequestReasonAccidentTranslations",
                maxLength: 60,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "TrucksTypesTranslations");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "TransportTypesTranslations");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "ShippingRequestReasonAccidentTranslations");
        }
    }
}