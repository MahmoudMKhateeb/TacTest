using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class SubmitInvoices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BinaryObjectId",
                table: "GroupPeriods");

            migrationBuilder.DropColumn(
                name: "DemandFileContentType",
                table: "GroupPeriods");

            migrationBuilder.DropColumn(
                name: "DemandFileName",
                table: "GroupPeriods");

            migrationBuilder.DropColumn(
                name: "IsClaim",
                table: "GroupPeriods");

            migrationBuilder.DropColumn(
                name: "IsDemand",
                table: "GroupPeriods");

            migrationBuilder.AddColumn<string>(
                name: "DocumentContentType",
                table: "GroupPeriods",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DocumentId",
                table: "GroupPeriods",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentName",
                table: "GroupPeriods",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectedReason",
                table: "GroupPeriods",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Status",
                table: "GroupPeriods",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentContentType",
                table: "GroupPeriods");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "GroupPeriods");

            migrationBuilder.DropColumn(
                name: "DocumentName",
                table: "GroupPeriods");

            migrationBuilder.DropColumn(
                name: "RejectedReason",
                table: "GroupPeriods");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "GroupPeriods");

            migrationBuilder.AddColumn<Guid>(
                name: "BinaryObjectId",
                table: "GroupPeriods",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DemandFileContentType",
                table: "GroupPeriods",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DemandFileName",
                table: "GroupPeriods",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsClaim",
                table: "GroupPeriods",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDemand",
                table: "GroupPeriods",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
