using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class UpdateSR_Remove_Trailer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_Trailers_AssignedTrailerId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_AssignedTrailerId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "AssignedTrailerId",
                table: "ShippingRequests");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AssignedTrailerId",
                table: "ShippingRequests",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_AssignedTrailerId",
                table: "ShippingRequests",
                column: "AssignedTrailerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_Trailers_AssignedTrailerId",
                table: "ShippingRequests",
                column: "AssignedTrailerId",
                principalTable: "Trailers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
