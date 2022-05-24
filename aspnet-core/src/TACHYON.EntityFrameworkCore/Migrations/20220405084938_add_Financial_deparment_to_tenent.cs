using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace TACHYON.Migrations
{
    public partial class add_Financial_deparment_to_tenent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FinancialEmail",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinancialName",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinancialPhone",
                table: "AbpTenants",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinancialEmail",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "FinancialName",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "FinancialPhone",
                table: "AbpTenants");
        }
    }
}