using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class removevasValuecolumnfromshippingrequesttable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Vas",
                table: "ShippingRequests");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Vas",
                table: "ShippingRequests",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
