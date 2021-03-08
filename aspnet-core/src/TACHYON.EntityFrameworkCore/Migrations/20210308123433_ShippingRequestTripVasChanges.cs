using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class ShippingRequestTripVasChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTripVases_ShippingRequestTripVases_ShippingRequestVasFkId",
                table: "ShippingRequestTripVases");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTripVases_ShippingRequestVasFkId",
                table: "ShippingRequestTripVases");

            migrationBuilder.DropColumn(
                name: "ShippingRequestVasFkId",
                table: "ShippingRequestTripVases");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ShippingRequestVasFkId",
                table: "ShippingRequestTripVases",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTripVases_ShippingRequestVasFkId",
                table: "ShippingRequestTripVases",
                column: "ShippingRequestVasFkId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTripVases_ShippingRequestTripVases_ShippingRequestVasFkId",
                table: "ShippingRequestTripVases",
                column: "ShippingRequestVasFkId",
                principalTable: "ShippingRequestTripVases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
