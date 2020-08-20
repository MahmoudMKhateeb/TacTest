using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addFatherShippingRequestIdtoShippingRequeststable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "FatherShippingRequestId",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_FatherShippingRequestId",
                table: "ShippingRequests",
                column: "FatherShippingRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_ShippingRequests_FatherShippingRequestId",
                table: "ShippingRequests",
                column: "FatherShippingRequestId",
                principalTable: "ShippingRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_ShippingRequests_FatherShippingRequestId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_FatherShippingRequestId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "FatherShippingRequestId",
                table: "ShippingRequests");
        }
    }
}
