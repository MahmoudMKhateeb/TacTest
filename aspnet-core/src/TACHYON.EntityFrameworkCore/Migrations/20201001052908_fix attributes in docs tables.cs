using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class fixattributesindocstables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "HasNotes",
                table: "DocumentFiles");

            migrationBuilder.DropColumn(
                name: "HasNumber",
                table: "DocumentFiles");

            migrationBuilder.AddColumn<bool>(
                name: "HasNotes",
                table: "DocumentTypes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasNumber",
                table: "DocumentTypes",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasNotes",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "HasNumber",
                table: "DocumentTypes");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDate",
                table: "DocumentTypes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "HasNotes",
                table: "DocumentFiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasNumber",
                table: "DocumentFiles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
