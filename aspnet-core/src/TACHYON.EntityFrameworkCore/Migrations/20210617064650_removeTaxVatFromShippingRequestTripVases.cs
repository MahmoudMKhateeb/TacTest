using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class removeTaxVatFromShippingRequestTripVases : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxVat",
                table: "ShippingRequestTripVases");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TaxVat",
                table: "ShippingRequestTripVases",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
