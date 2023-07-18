using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class report_file_and_publish : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GeneratedFileId",
                table: "Reports",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Reports",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Reports_GeneratedFileId",
                table: "Reports",
                column: "GeneratedFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AppBinaryObjects_GeneratedFileId",
                table: "Reports",
                column: "GeneratedFileId",
                principalTable: "AppBinaryObjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AppBinaryObjects_GeneratedFileId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_GeneratedFileId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "GeneratedFileId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Reports");
        }
    }
}
