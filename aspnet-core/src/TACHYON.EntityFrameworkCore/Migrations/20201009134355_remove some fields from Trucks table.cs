using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class removesomefieldsfromTruckstable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trucks_AbpUsers_Driver2UserId",
                table: "Trucks");

            migrationBuilder.DropIndex(
                name: "IX_Trucks_Driver2UserId",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "Driver2UserId",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "LicenseExpirationDate",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "LicenseNumber",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "RentDuration",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "RentPrice",
                table: "Trucks");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Driver2UserId",
                table: "Trucks",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LicenseExpirationDate",
                table: "Trucks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "LicenseNumber",
                table: "Trucks",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RentDuration",
                table: "Trucks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RentPrice",
                table: "Trucks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_Driver2UserId",
                table: "Trucks",
                column: "Driver2UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trucks_AbpUsers_Driver2UserId",
                table: "Trucks",
                column: "Driver2UserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
