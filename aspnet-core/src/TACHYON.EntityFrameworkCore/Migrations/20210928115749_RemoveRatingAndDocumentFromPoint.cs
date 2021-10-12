using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace TACHYON.Migrations
{
    public partial class RemoveRatingAndDocumentFromPoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentContentType",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "DocumentName",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "RoutPoints");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DocumentContentType",
                table: "RoutPoints",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DocumentId",
                table: "RoutPoints",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentName",
                table: "RoutPoints",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "RoutPoints",
                type: "float",
                nullable: true);
        }
    }
}