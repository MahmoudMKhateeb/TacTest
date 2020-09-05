using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addShippingRequestStatusIdtoShippingRequeststable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShippingRequestStatusId",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_ShippingRequestStatusId",
                table: "ShippingRequests",
                column: "ShippingRequestStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_ShippingRequestStatuses_ShippingRequestStatusId",
                table: "ShippingRequests",
                column: "ShippingRequestStatusId",
                principalTable: "ShippingRequestStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_ShippingRequestStatuses_ShippingRequestStatusId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_ShippingRequestStatusId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "ShippingRequestStatusId",
                table: "ShippingRequests");
        }
    }
}
