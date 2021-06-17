using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class AddShippingRequestTriptoDocumentFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShippingRequestTripId",
                table: "DocumentFiles",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFiles_ShippingRequestTripId",
                table: "DocumentFiles",
                column: "ShippingRequestTripId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentFiles_ShippingRequestTrips_ShippingRequestTripId",
                table: "DocumentFiles",
                column: "ShippingRequestTripId",
                principalTable: "ShippingRequestTrips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentFiles_ShippingRequestTrips_ShippingRequestTripId",
                table: "DocumentFiles");

            migrationBuilder.DropIndex(
                name: "IX_DocumentFiles_ShippingRequestTripId",
                table: "DocumentFiles");

            migrationBuilder.DropColumn(
                name: "ShippingRequestTripId",
                table: "DocumentFiles");
        }
    }
}
