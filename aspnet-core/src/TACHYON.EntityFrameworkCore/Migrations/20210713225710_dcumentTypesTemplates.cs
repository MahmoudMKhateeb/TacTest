using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class dcumentTypesTemplates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DocumentRelatedWithId",
                table: "DocumentTypes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TemplateContentType",
                table: "DocumentTypes",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TemplateId",
                table: "DocumentTypes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TemplateName",
                table: "DocumentTypes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentRelatedWithId",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "TemplateContentType",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "TemplateId",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "TemplateName",
                table: "DocumentTypes");
        }
    }
}
