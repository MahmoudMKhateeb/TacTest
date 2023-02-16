using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class sass_shipments_updates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTrips_ShippingRequests_ShippingRequestId",
                table: "ShippingRequestTrips");

            migrationBuilder.AlterColumn<long>(
                name: "ShippingRequestId",
                table: "ShippingRequestTrips",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<int>(
                name: "CarrierTenantId",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GoodCategoryId",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShipperTenantId",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_CarrierTenantId",
                table: "ShippingRequestTrips",
                column: "CarrierTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_GoodCategoryId",
                table: "ShippingRequestTrips",
                column: "GoodCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_ShipperTenantId",
                table: "ShippingRequestTrips",
                column: "ShipperTenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTrips_AbpTenants_CarrierTenantId",
                table: "ShippingRequestTrips",
                column: "CarrierTenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTrips_GoodCategories_GoodCategoryId",
                table: "ShippingRequestTrips",
                column: "GoodCategoryId",
                principalTable: "GoodCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTrips_AbpTenants_ShipperTenantId",
                table: "ShippingRequestTrips",
                column: "ShipperTenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTrips_ShippingRequests_ShippingRequestId",
                table: "ShippingRequestTrips",
                column: "ShippingRequestId",
                principalTable: "ShippingRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTrips_AbpTenants_CarrierTenantId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTrips_GoodCategories_GoodCategoryId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTrips_AbpTenants_ShipperTenantId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTrips_ShippingRequests_ShippingRequestId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTrips_CarrierTenantId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTrips_GoodCategoryId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTrips_ShipperTenantId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "CarrierTenantId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "GoodCategoryId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "ShipperTenantId",
                table: "ShippingRequestTrips");

            migrationBuilder.AlterColumn<long>(
                name: "ShippingRequestId",
                table: "ShippingRequestTrips",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTrips_ShippingRequests_ShippingRequestId",
                table: "ShippingRequestTrips",
                column: "ShippingRequestId",
                principalTable: "ShippingRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
