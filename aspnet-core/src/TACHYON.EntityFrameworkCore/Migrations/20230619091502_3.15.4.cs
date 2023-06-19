using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class _3154 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTaxVatIncluded",
                table: "InvoiceNotes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "StampFileType",
                table: "AbpTenants",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StampId",
                table: "AbpTenants",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTaxVatIncluded",
                table: "InvoiceNotes");

            migrationBuilder.DropColumn(
                name: "StampFileType",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "StampId",
                table: "AbpTenants");
        }
    }
}
