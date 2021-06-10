using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class SetSourceIdAcceptNullinPricing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestPricings_ShippingRequestDirectRequests_ShippingRequestDirectRequestId",
                table: "ShippingRequestPricings");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestPricings_ShippingRequestDirectRequestId",
                table: "ShippingRequestPricings");

            migrationBuilder.DropColumn(
                name: "ShippingRequestDirectRequestId",
                table: "ShippingRequestPricings");

            migrationBuilder.AlterColumn<long>(
                name: "SourceId",
                table: "ShippingRequestPricings",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "SourceId",
                table: "ShippingRequestPricings",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ShippingRequestDirectRequestId",
                table: "ShippingRequestPricings",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestPricings_ShippingRequestDirectRequestId",
                table: "ShippingRequestPricings",
                column: "ShippingRequestDirectRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestPricings_ShippingRequestDirectRequests_ShippingRequestDirectRequestId",
                table: "ShippingRequestPricings",
                column: "ShippingRequestDirectRequestId",
                principalTable: "ShippingRequestDirectRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
