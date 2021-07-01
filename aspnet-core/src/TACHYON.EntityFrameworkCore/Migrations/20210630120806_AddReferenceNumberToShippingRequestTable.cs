using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddReferenceNumberToShippingRequestTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ReferenceNumber",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_ReferenceNumber",
                table: "ShippingRequests",
                column: "ReferenceNumber",
                unique: true,
                filter: "[ReferenceNumber] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_ReferenceNumber",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "ReferenceNumber",
                table: "ShippingRequests");
        }
    }
}
