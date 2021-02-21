using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class GroupPeriodChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsAccountReceivable",
                table: "Invoices",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BinaryObjectId",
                table: "GroupPeriods",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsClaims",
                table: "GroupPeriods",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BinaryObjectId",
                table: "GroupPeriods");

            migrationBuilder.DropColumn(
                name: "IsClaims",
                table: "GroupPeriods");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAccountReceivable",
                table: "Invoices",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool));
        }
    }
}
