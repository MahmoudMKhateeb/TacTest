using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Add_OldPriceOfferId_In_ShippingRequestUpdate_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "OldPriceOfferId",
                table: "ShippingRequestUpdates",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestUpdates_OldPriceOfferId",
                table: "ShippingRequestUpdates",
                column: "OldPriceOfferId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestUpdates_PriceOffers_OldPriceOfferId",
                table: "ShippingRequestUpdates",
                column: "OldPriceOfferId",
                principalTable: "PriceOffers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestUpdates_PriceOffers_OldPriceOfferId",
                table: "ShippingRequestUpdates");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestUpdates_OldPriceOfferId",
                table: "ShippingRequestUpdates");

            migrationBuilder.DropColumn(
                name: "OldPriceOfferId",
                table: "ShippingRequestUpdates");
        }
    }
}
