using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addTrucksTypeIdTrailerTypeIdtoShippingRequesttable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TrailerTypeId",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TrucksTypeId",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_TrailerTypeId",
                table: "ShippingRequests",
                column: "TrailerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_TrucksTypeId",
                table: "ShippingRequests",
                column: "TrucksTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_TrailerTypes_TrailerTypeId",
                table: "ShippingRequests",
                column: "TrailerTypeId",
                principalTable: "TrailerTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_TrucksTypes_TrucksTypeId",
                table: "ShippingRequests",
                column: "TrucksTypeId",
                principalTable: "TrucksTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_TrailerTypes_TrailerTypeId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_TrucksTypes_TrucksTypeId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_TrailerTypeId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_TrucksTypeId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "TrailerTypeId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "TrucksTypeId",
                table: "ShippingRequests");
        }
    }
}
