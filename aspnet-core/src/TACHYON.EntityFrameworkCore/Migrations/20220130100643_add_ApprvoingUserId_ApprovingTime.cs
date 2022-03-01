using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class add_ApprvoingUserId_ApprovingTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovingTime",
                table: "PriceOffers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ApprovingUserId",
                table: "PriceOffers",
                nullable: false,
                defaultValue: 0);
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovingTime",
                table: "PriceOffers");

            migrationBuilder.DropColumn(
                name: "ApprovingUserId",
                table: "PriceOffers");
        }
    }
}
