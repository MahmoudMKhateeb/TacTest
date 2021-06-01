using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class ShippingRequestPricingsAddNewFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestPricings_ShippingRequests_ShippingRequest",
                table: "ShippingRequestPricings");

            migrationBuilder.DropColumn(
                name: "Commission",
                table: "ShippingRequestPricings");

            migrationBuilder.AlterColumn<long>(
                name: "ShippingRequest",
                table: "ShippingRequestPricings",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionAmount",
                table: "ShippingRequestPricings",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsView",
                table: "ShippingRequestPricings",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "ShippingRequestId",
                table: "ShippingRequestPricings",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalAmountWithCommission",
                table: "ShippingRequestPricings",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmountWithCommission",
                table: "ShippingRequestPricings",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TripCommissionAmount",
                table: "ShippingRequestPricings",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TripSubTotalAmountWithCommission",
                table: "ShippingRequestPricings",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TripTotalAmountWithCommission",
                table: "ShippingRequestPricings",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TripVatAmountWithCommission",
                table: "ShippingRequestPricings",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VatAmountWithCommission",
                table: "ShippingRequestPricings",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestPricings_ShippingRequests_ShippingRequest",
                table: "ShippingRequestPricings",
                column: "ShippingRequest",
                principalTable: "ShippingRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestPricings_ShippingRequests_ShippingRequest",
                table: "ShippingRequestPricings");

            migrationBuilder.DropColumn(
                name: "CommissionAmount",
                table: "ShippingRequestPricings");

            migrationBuilder.DropColumn(
                name: "IsView",
                table: "ShippingRequestPricings");

            migrationBuilder.DropColumn(
                name: "ShippingRequestId",
                table: "ShippingRequestPricings");

            migrationBuilder.DropColumn(
                name: "SubTotalAmountWithCommission",
                table: "ShippingRequestPricings");

            migrationBuilder.DropColumn(
                name: "TotalAmountWithCommission",
                table: "ShippingRequestPricings");

            migrationBuilder.DropColumn(
                name: "TripCommissionAmount",
                table: "ShippingRequestPricings");

            migrationBuilder.DropColumn(
                name: "TripSubTotalAmountWithCommission",
                table: "ShippingRequestPricings");

            migrationBuilder.DropColumn(
                name: "TripTotalAmountWithCommission",
                table: "ShippingRequestPricings");

            migrationBuilder.DropColumn(
                name: "TripVatAmountWithCommission",
                table: "ShippingRequestPricings");

            migrationBuilder.DropColumn(
                name: "VatAmountWithCommission",
                table: "ShippingRequestPricings");

            migrationBuilder.AlterColumn<long>(
                name: "ShippingRequest",
                table: "ShippingRequestPricings",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Commission",
                table: "ShippingRequestPricings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestPricings_ShippingRequests_ShippingRequest",
                table: "ShippingRequestPricings",
                column: "ShippingRequest",
                principalTable: "ShippingRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
