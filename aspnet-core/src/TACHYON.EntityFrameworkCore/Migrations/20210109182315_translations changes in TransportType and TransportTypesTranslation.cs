using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class translationschangesinTransportTypeandTransportTypesTranslation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "TransportTypesTranslations");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "TransportTypes");

            migrationBuilder.AddColumn<string>(
                name: "TranslatedDisplayName",
                table: "TransportTypesTranslations",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "TransportTypes",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TranslatedDisplayName",
                table: "TransportTypesTranslations");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "TransportTypes");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "TransportTypesTranslations",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "TransportTypes",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }
    }
}
