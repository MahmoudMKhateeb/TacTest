using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class PenaltyPricesUpdate_PenaltyTB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VatPreCommestion",
                table: "Penalties");

            migrationBuilder.AddColumn<decimal>(
                name: "TaxVat",
                table: "Penalties",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxVat",
                table: "Penalties");

            migrationBuilder.AddColumn<decimal>(
                name: "VatPreCommestion",
                table: "Penalties",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
