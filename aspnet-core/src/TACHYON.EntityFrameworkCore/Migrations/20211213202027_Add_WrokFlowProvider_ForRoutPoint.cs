using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace TACHYON.Migrations
{
    public partial class Add_WrokFlowProvider_ForRoutPoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Language",
                table: "TrucksTypesTranslations",
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32);

            migrationBuilder.AddColumn<bool>(
                name: "IsReset",
                table: "RoutPointStatusTransitions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanGoToNextLocation",
                table: "RoutPoints",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPodUploaded",
                table: "RoutPoints",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsResolve",
                table: "RoutPoints",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "WorkFlowVersion",
                table: "RoutPoints",
                nullable: false,
                defaultValue: 0);


            migrationBuilder.AddColumn<int>(
                name: "DriverStatus",
                table: "AbpUsers",
                nullable: true);


        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.DropColumn(
                name: "IsReset",
                table: "RoutPointStatusTransitions");

            migrationBuilder.DropColumn(
                name: "CanGoToNextLocation",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "IsPodUploaded",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "IsResolve",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "WorkFlowVersion",
                table: "RoutPoints");



            migrationBuilder.DropColumn(
                name: "ItemsTotalPricePreCommissionPreVat",
                table: "PriceOfferDetails");

            migrationBuilder.DropColumn(
                name: "DriverStatus",
                table: "AbpUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Language",
                table: "TrucksTypesTranslations",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 5);
        }
    }
}