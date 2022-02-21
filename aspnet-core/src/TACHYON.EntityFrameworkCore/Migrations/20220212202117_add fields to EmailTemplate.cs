using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addfieldstoEmailTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "EmailTemplates",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmailTemplateType",
                table: "EmailTemplates",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "EmailTemplates",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "EmailTemplates");

            migrationBuilder.DropColumn(
                name: "EmailTemplateType",
                table: "EmailTemplates");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "EmailTemplates");
        }
    }
}
