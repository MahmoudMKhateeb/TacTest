using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddIsMandatoryForDocuments_Columns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRequiredDocumentTemplate",
                table: "DocumentTypes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRequiredExpirationDate",
                table: "DocumentTypes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRequiredNumber",
                table: "DocumentTypes",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRequiredDocumentTemplate",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "IsRequiredExpirationDate",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "IsRequiredNumber",
                table: "DocumentTypes");
        }
    }
}