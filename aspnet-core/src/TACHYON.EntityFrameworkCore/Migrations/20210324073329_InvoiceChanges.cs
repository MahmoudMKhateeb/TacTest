using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class InvoiceChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreditLimit",
                table: "InvoicePeriods");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceNo",
                table: "BalanceRecharges",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferenceNo",
                table: "BalanceRecharges");

            migrationBuilder.AddColumn<double>(
                name: "CreditLimit",
                table: "InvoicePeriods",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
