using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class release_1_2_5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDrafted",
                table: "Penalties");

            migrationBuilder.DropColumn(
                name: "VatPreCommestion",
                table: "Penalties");

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "ShippingRequestTripRejectReasons",
                maxLength: 60,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "TaxVat",
                table: "Penalties",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsDrafted",
                table: "InvoiceNotes",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Key",
                table: "ShippingRequestTripRejectReasons");

            migrationBuilder.DropColumn(
                name: "TaxVat",
                table: "Penalties");

            migrationBuilder.DropColumn(
                name: "IsDrafted",
                table: "InvoiceNotes");

            migrationBuilder.AddColumn<bool>(
                name: "IsDrafted",
                table: "Penalties",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "VatPreCommestion",
                table: "Penalties",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
