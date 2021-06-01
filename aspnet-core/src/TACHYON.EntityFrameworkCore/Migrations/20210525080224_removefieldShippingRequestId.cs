using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class removefieldShippingRequestId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestPricings_ShippingRequests_ShippingRequest",
                table: "ShippingRequestPricings");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestPricings_ShippingRequest",
                table: "ShippingRequestPricings");

            migrationBuilder.DropColumn(
                name: "ShippingRequest",
                table: "ShippingRequestPricings");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestPricings_ShippingRequestId",
                table: "ShippingRequestPricings",
                column: "ShippingRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestPricings_ShippingRequests_ShippingRequestId",
                table: "ShippingRequestPricings",
                column: "ShippingRequestId",
                principalTable: "ShippingRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestPricings_ShippingRequests_ShippingRequestId",
                table: "ShippingRequestPricings");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestPricings_ShippingRequestId",
                table: "ShippingRequestPricings");

            migrationBuilder.AddColumn<long>(
                name: "ShippingRequest",
                table: "ShippingRequestPricings",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestPricings_ShippingRequest",
                table: "ShippingRequestPricings",
                column: "ShippingRequest");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestPricings_ShippingRequests_ShippingRequest",
                table: "ShippingRequestPricings",
                column: "ShippingRequest",
                principalTable: "ShippingRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
