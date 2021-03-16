using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Shipperbalance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_InvoicePeriods_InvoicePeriodId",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_InvoicePeriodId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "InvoicePeriodId",
                table: "AbpTenants");

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "AbpTenants",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "AbpTenants");

            migrationBuilder.AddColumn<int>(
                name: "InvoicePeriodId",
                table: "AbpTenants",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_InvoicePeriodId",
                table: "AbpTenants",
                column: "InvoicePeriodId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_InvoicePeriods_InvoicePeriodId",
                table: "AbpTenants",
                column: "InvoicePeriodId",
                principalTable: "InvoicePeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
