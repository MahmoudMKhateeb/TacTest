using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class release_2_4_18 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Commission",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "CommissionType",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "TmsPricePackages");

            migrationBuilder.DropColumn(
                name: "ContractDate",
                table: "PricePackageAppendixes");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "PricePackageAppendixes");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "PricePackageAppendixes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MajorVersion",
                table: "PricePackageAppendixes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinorVersion",
                table: "PricePackageAppendixes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AppendixId",
                table: "NormalPricePackages",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NormalPricePackages_AppendixId",
                table: "NormalPricePackages",
                column: "AppendixId");

            migrationBuilder.AddForeignKey(
                name: "FK_NormalPricePackages_PricePackageAppendixes_AppendixId",
                table: "NormalPricePackages",
                column: "AppendixId",
                principalTable: "PricePackageAppendixes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NormalPricePackages_PricePackageAppendixes_AppendixId",
                table: "NormalPricePackages");

            migrationBuilder.DropIndex(
                name: "IX_NormalPricePackages_AppendixId",
                table: "NormalPricePackages");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "PricePackageAppendixes");

            migrationBuilder.DropColumn(
                name: "MajorVersion",
                table: "PricePackageAppendixes");

            migrationBuilder.DropColumn(
                name: "MinorVersion",
                table: "PricePackageAppendixes");

            migrationBuilder.DropColumn(
                name: "AppendixId",
                table: "NormalPricePackages");

            migrationBuilder.AddColumn<decimal>(
                name: "Commission",
                table: "TmsPricePackages",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "CommissionType",
                table: "TmsPricePackages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TmsPricePackages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "TmsPricePackages",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "ContractDate",
                table: "PricePackageAppendixes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "PricePackageAppendixes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
