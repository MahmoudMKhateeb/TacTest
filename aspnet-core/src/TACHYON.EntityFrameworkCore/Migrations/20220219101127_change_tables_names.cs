using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class change_tables_names : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestDirectRequests_BidNormalPricePackages_BidNormalPricePackageId",
                table: "ShippingRequestDirectRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestDirectRequests_BidNormalPricePackageId",
                table: "ShippingRequestDirectRequests");

            migrationBuilder.DropColumn(
                name: "BidNormalPricePackageId",
                table: "ShippingRequestDirectRequests");

            migrationBuilder.AddColumn<long>(
                name: "PricePackageOfferId",
                table: "ShippingRequestDirectRequests",
                nullable: true);


            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestDirectRequests_PricePackageOfferId",
                table: "ShippingRequestDirectRequests",
                column: "PricePackageOfferId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestDirectRequests_BidNormalPricePackages_PricePackageOfferId",
                table: "ShippingRequestDirectRequests",
                column: "PricePackageOfferId",
                principalTable: "BidNormalPricePackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestDirectRequests_BidNormalPricePackages_PricePackageOfferId",
                table: "ShippingRequestDirectRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestDirectRequests_PricePackageOfferId",
                table: "ShippingRequestDirectRequests");

            migrationBuilder.DropColumn(
                name: "PricePackageOfferId",
                table: "ShippingRequestDirectRequests");


            migrationBuilder.AddColumn<long>(
                name: "BidNormalPricePackageId",
                table: "ShippingRequestDirectRequests",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestDirectRequests_BidNormalPricePackageId",
                table: "ShippingRequestDirectRequests",
                column: "BidNormalPricePackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestDirectRequests_BidNormalPricePackages_BidNormalPricePackageId",
                table: "ShippingRequestDirectRequests",
                column: "BidNormalPricePackageId",
                principalTable: "BidNormalPricePackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}