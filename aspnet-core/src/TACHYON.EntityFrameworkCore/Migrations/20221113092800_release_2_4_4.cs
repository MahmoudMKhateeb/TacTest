using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class release_2_4_4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActorCarrierPrice_ShippingRequestTrips_ShippingRequestTripId",
                table: "ActorCarrierPrice");

            migrationBuilder.DropForeignKey(
                name: "FK_ActorCarrierPrice_ShippingRequestTripVases_ShippingRequestTripVasId",
                table: "ActorCarrierPrice");

            migrationBuilder.DropForeignKey(
                name: "FK_ActorShipperPrices_ShippingRequestTrips_ShippingRequestTripId",
                table: "ActorShipperPrices");

            migrationBuilder.DropForeignKey(
                name: "FK_ActorShipperPrices_ShippingRequestTripVases_ShippingRequestTripVasId",
                table: "ActorShipperPrices");

            migrationBuilder.DropIndex(
                name: "IX_ActorShipperPrices_ShippingRequestTripId",
                table: "ActorShipperPrices");

            migrationBuilder.DropIndex(
                name: "IX_ActorShipperPrices_ShippingRequestTripVasId",
                table: "ActorShipperPrices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActorCarrierPrice",
                table: "ActorCarrierPrice");

            migrationBuilder.DropIndex(
                name: "IX_ActorCarrierPrice_ShippingRequestTripId",
                table: "ActorCarrierPrice");

            migrationBuilder.DropIndex(
                name: "IX_ActorCarrierPrice_ShippingRequestTripVasId",
                table: "ActorCarrierPrice");

            migrationBuilder.DropColumn(
                name: "IsActorShipperHaveInvoice",
                table: "ActorShipperPrices");

            migrationBuilder.DropColumn(
                name: "ShippingRequestTripId",
                table: "ActorShipperPrices");

            migrationBuilder.DropColumn(
                name: "ShippingRequestTripVasId",
                table: "ActorShipperPrices");

            migrationBuilder.DropColumn(
                name: "ShippingRequestTripId",
                table: "ActorCarrierPrice");

            migrationBuilder.DropColumn(
                name: "ShippingRequestTripVasId",
                table: "ActorCarrierPrice");

            migrationBuilder.RenameTable(
                name: "ActorCarrierPrice",
                newName: "ActorCarrierPrices");

            migrationBuilder.AddColumn<int>(
                name: "ActorCarrierPriceId",
                table: "ShippingRequestVases",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ActorShipperPriceId",
                table: "ShippingRequestVases",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActorCarrierHaveInvoice",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActorShipperHaveInvoice",
                table: "ShippingRequestTrips",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ActorCarrierPriceId",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ActorShipperPriceId",
                table: "ShippingRequests",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ShippingRequestId",
                table: "ActorShipperPrices",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ShippingRequestVasId",
                table: "ActorShipperPrices",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ShippingRequestId",
                table: "ActorCarrierPrices",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ShippingRequestVasId",
                table: "ActorCarrierPrices",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActorCarrierPrices",
                table: "ActorCarrierPrices",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestVases_ActorCarrierPriceId",
                table: "ShippingRequestVases",
                column: "ActorCarrierPriceId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequestVases_ActorShipperPriceId",
                table: "ShippingRequestVases",
                column: "ActorShipperPriceId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_ActorCarrierPriceId",
                table: "ShippingRequests",
                column: "ActorCarrierPriceId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_ActorShipperPriceId",
                table: "ShippingRequests",
                column: "ActorShipperPriceId");

            migrationBuilder.CreateIndex(
                name: "IX_ActorShipperPrices_ShippingRequestId",
                table: "ActorShipperPrices",
                column: "ShippingRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ActorShipperPrices_ShippingRequestVasId",
                table: "ActorShipperPrices",
                column: "ShippingRequestVasId");

            migrationBuilder.CreateIndex(
                name: "IX_ActorCarrierPrices_ShippingRequestId",
                table: "ActorCarrierPrices",
                column: "ShippingRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ActorCarrierPrices_ShippingRequestVasId",
                table: "ActorCarrierPrices",
                column: "ShippingRequestVasId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActorCarrierPrices_ShippingRequests_ShippingRequestId",
                table: "ActorCarrierPrices",
                column: "ShippingRequestId",
                principalTable: "ShippingRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ActorCarrierPrices_ShippingRequestVases_ShippingRequestVasId",
                table: "ActorCarrierPrices",
                column: "ShippingRequestVasId",
                principalTable: "ShippingRequestVases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ActorShipperPrices_ShippingRequests_ShippingRequestId",
                table: "ActorShipperPrices",
                column: "ShippingRequestId",
                principalTable: "ShippingRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ActorShipperPrices_ShippingRequestVases_ShippingRequestVasId",
                table: "ActorShipperPrices",
                column: "ShippingRequestVasId",
                principalTable: "ShippingRequestVases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_ActorCarrierPrices_ActorCarrierPriceId",
                table: "ShippingRequests",
                column: "ActorCarrierPriceId",
                principalTable: "ActorCarrierPrices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_ActorShipperPrices_ActorShipperPriceId",
                table: "ShippingRequests",
                column: "ActorShipperPriceId",
                principalTable: "ActorShipperPrices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestVases_ActorCarrierPrices_ActorCarrierPriceId",
                table: "ShippingRequestVases",
                column: "ActorCarrierPriceId",
                principalTable: "ActorCarrierPrices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequestVases_ActorShipperPrices_ActorShipperPriceId",
                table: "ShippingRequestVases",
                column: "ActorShipperPriceId",
                principalTable: "ActorShipperPrices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActorCarrierPrices_ShippingRequests_ShippingRequestId",
                table: "ActorCarrierPrices");

            migrationBuilder.DropForeignKey(
                name: "FK_ActorCarrierPrices_ShippingRequestVases_ShippingRequestVasId",
                table: "ActorCarrierPrices");

            migrationBuilder.DropForeignKey(
                name: "FK_ActorShipperPrices_ShippingRequests_ShippingRequestId",
                table: "ActorShipperPrices");

            migrationBuilder.DropForeignKey(
                name: "FK_ActorShipperPrices_ShippingRequestVases_ShippingRequestVasId",
                table: "ActorShipperPrices");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_ActorCarrierPrices_ActorCarrierPriceId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_ActorShipperPrices_ActorShipperPriceId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestVases_ActorCarrierPrices_ActorCarrierPriceId",
                table: "ShippingRequestVases");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequestVases_ActorShipperPrices_ActorShipperPriceId",
                table: "ShippingRequestVases");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestVases_ActorCarrierPriceId",
                table: "ShippingRequestVases");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequestVases_ActorShipperPriceId",
                table: "ShippingRequestVases");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_ActorCarrierPriceId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_ActorShipperPriceId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ActorShipperPrices_ShippingRequestId",
                table: "ActorShipperPrices");

            migrationBuilder.DropIndex(
                name: "IX_ActorShipperPrices_ShippingRequestVasId",
                table: "ActorShipperPrices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActorCarrierPrices",
                table: "ActorCarrierPrices");

            migrationBuilder.DropIndex(
                name: "IX_ActorCarrierPrices_ShippingRequestId",
                table: "ActorCarrierPrices");

            migrationBuilder.DropIndex(
                name: "IX_ActorCarrierPrices_ShippingRequestVasId",
                table: "ActorCarrierPrices");

            migrationBuilder.DropColumn(
                name: "ActorCarrierPriceId",
                table: "ShippingRequestVases");

            migrationBuilder.DropColumn(
                name: "ActorShipperPriceId",
                table: "ShippingRequestVases");

            migrationBuilder.DropColumn(
                name: "IsActorCarrierHaveInvoice",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "IsActorShipperHaveInvoice",
                table: "ShippingRequestTrips");

            migrationBuilder.DropColumn(
                name: "ActorCarrierPriceId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "ActorShipperPriceId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "ShippingRequestId",
                table: "ActorShipperPrices");

            migrationBuilder.DropColumn(
                name: "ShippingRequestVasId",
                table: "ActorShipperPrices");

            migrationBuilder.DropColumn(
                name: "ShippingRequestId",
                table: "ActorCarrierPrices");

            migrationBuilder.DropColumn(
                name: "ShippingRequestVasId",
                table: "ActorCarrierPrices");

            migrationBuilder.RenameTable(
                name: "ActorCarrierPrices",
                newName: "ActorCarrierPrice");

            migrationBuilder.AddColumn<bool>(
                name: "IsActorShipperHaveInvoice",
                table: "ActorShipperPrices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ShippingRequestTripId",
                table: "ActorShipperPrices",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ShippingRequestTripVasId",
                table: "ActorShipperPrices",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShippingRequestTripId",
                table: "ActorCarrierPrice",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ShippingRequestTripVasId",
                table: "ActorCarrierPrice",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActorCarrierPrice",
                table: "ActorCarrierPrice",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ActorShipperPrices_ShippingRequestTripId",
                table: "ActorShipperPrices",
                column: "ShippingRequestTripId",
                unique: true,
                filter: "[ShippingRequestTripId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ActorShipperPrices_ShippingRequestTripVasId",
                table: "ActorShipperPrices",
                column: "ShippingRequestTripVasId",
                unique: true,
                filter: "[ShippingRequestTripVasId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ActorCarrierPrice_ShippingRequestTripId",
                table: "ActorCarrierPrice",
                column: "ShippingRequestTripId",
                unique: true,
                filter: "[ShippingRequestTripId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ActorCarrierPrice_ShippingRequestTripVasId",
                table: "ActorCarrierPrice",
                column: "ShippingRequestTripVasId",
                unique: true,
                filter: "[ShippingRequestTripVasId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ActorCarrierPrice_ShippingRequestTrips_ShippingRequestTripId",
                table: "ActorCarrierPrice",
                column: "ShippingRequestTripId",
                principalTable: "ShippingRequestTrips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ActorCarrierPrice_ShippingRequestTripVases_ShippingRequestTripVasId",
                table: "ActorCarrierPrice",
                column: "ShippingRequestTripVasId",
                principalTable: "ShippingRequestTripVases",
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
                name: "FK_ActorShipperPrices_ShippingRequestTripVases_ShippingRequestTripVasId",
                table: "ActorShipperPrices",
                column: "ShippingRequestTripVasId",
                principalTable: "ShippingRequestTripVases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
