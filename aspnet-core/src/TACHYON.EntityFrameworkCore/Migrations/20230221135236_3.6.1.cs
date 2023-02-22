using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class _361 : Migration
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
                name: "CarrierActorId",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CarrierTenantId",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GoodCategoryId",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShipperActorId",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShipperTenantId",
                table: "ShippingRequestTrips",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShippingRequestTripId",
                table: "ActorShipperPrices",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShippingRequestTripId",
                table: "ActorCarrierPrices",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_CarrierActorId",
                table: "ShippingRequestTrips",
                column: "CarrierActorId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_CarrierTenantId",
                table: "ShippingRequestTrips",
                column: "CarrierTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_GoodCategoryId",
                table: "ShippingRequestTrips",
                column: "GoodCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_ShipperActorId",
                table: "ShippingRequestTrips",
                column: "ShipperActorId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestTrips_ShipperTenantId",
                table: "ShippingRequestTrips",
                column: "ShipperTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ActorShipperPrices_ShippingRequestTripId",
                table: "ActorShipperPrices",
                column: "ShippingRequestTripId",
                unique: true,
                filter: "[ShippingRequestTripId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ActorCarrierPrices_ShippingRequestTripId",
                table: "ActorCarrierPrices",
                column: "ShippingRequestTripId",
                unique: true,
                filter: "[ShippingRequestTripId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ActorCarrierPrices_ShippingRequestTrips_ShippingRequestTripId",
                table: "ActorCarrierPrices",
                column: "ShippingRequestTripId",
                principalTable: "ShippingRequestTrips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ActorShipperPrices_ShippingRequestTrips_ShippingRequestTripId",
                table: "ActorShipperPrices",
                column: "ShippingRequestTripId",
                principalTable: "ShippingRequestTrips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestTrips_Actors_CarrierActorId",
                table: "ShippingRequestTrips",
                column: "CarrierActorId",
                principalTable: "Actors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
                name: "FK_ShippingRequestTrips_Actors_ShipperActorId",
                table: "ShippingRequestTrips",
                column: "ShipperActorId",
                principalTable: "Actors",
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
                name: "FK_ActorCarrierPrices_ShippingRequestTrips_ShippingRequestTripId",
                table: "ActorCarrierPrices");

            migrationBuilder.DropForeignKey(
                name: "FK_ActorShipperPrices_ShippingRequestTrips_ShippingRequestTripId",
                table: "ActorShipperPrices");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTrips_Actors_CarrierActorId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTrips_AbpTenants_CarrierTenantId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTrips_GoodCategories_GoodCategoryId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTrips_Actors_ShipperActorId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTrips_AbpTenants_ShipperTenantId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestTrips_ShippingRequests_ShippingRequestId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTrips_CarrierActorId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTrips_CarrierTenantId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTrips_GoodCategoryId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTrips_ShipperActorId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestTrips_ShipperTenantId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropIndex(
                name: "IX_ActorShipperPrices_ShippingRequestTripId",
                table: "ActorShipperPrices");

            migrationBuilder.DropIndex(
                name: "IX_ActorCarrierPrices_ShippingRequestTripId",
                table: "ActorCarrierPrices");

            migrationBuilder.DropColumn(
                name: "CarrierActorId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "CarrierTenantId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "GoodCategoryId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "ShipperActorId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "ShipperTenantId",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "ShippingRequestTripId",
                table: "ActorShipperPrices");

            migrationBuilder.DropColumn(
                name: "ShippingRequestTripId",
                table: "ActorCarrierPrices");

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
