using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class renameIsAcceptedtoIsPriceAcceptedinShippingRequesttable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAccepted",
                table: "ShippingRequests");

            migrationBuilder.AddColumn<bool>(
                name: "IsPriceAccepted",
                table: "ShippingRequests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPriceAccepted",
                table: "ShippingRequests");

            migrationBuilder.AddColumn<bool>(
                name: "IsAccepted",
                table: "ShippingRequests",
                type: "bit",
                nullable: true);
        }
    }
}
