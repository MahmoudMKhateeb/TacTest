using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class moveshippingdetailsfromshippingrequesttotoroutestepstable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_GoodsDetails_GoodsDetailId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_TrailerTypes_TrailerTypeId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_TrucksTypes_TrucksTypeId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_GoodsDetailId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_TrailerTypeId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_TrucksTypeId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "GoodsDetailId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "TrailerTypeId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "TrucksTypeId",
                table: "ShippingRequests");

            migrationBuilder.AddColumn<long>(
                name: "GoodsDetailId",
                table: "RoutSteps",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrailerTypeId",
                table: "RoutSteps",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TrucksTypeId",
                table: "RoutSteps",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoutSteps_GoodsDetailId",
                table: "RoutSteps",
                column: "GoodsDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutSteps_TrailerTypeId",
                table: "RoutSteps",
                column: "TrailerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutSteps_TrucksTypeId",
                table: "RoutSteps",
                column: "TrucksTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_GoodsDetails_GoodsDetailId",
                table: "RoutSteps",
                column: "GoodsDetailId",
                principalTable: "GoodsDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_TrailerTypes_TrailerTypeId",
                table: "RoutSteps",
                column: "TrailerTypeId",
                principalTable: "TrailerTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_TrucksTypes_TrucksTypeId",
                table: "RoutSteps",
                column: "TrucksTypeId",
                principalTable: "TrucksTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_GoodsDetails_GoodsDetailId",
                table: "RoutSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_TrailerTypes_TrailerTypeId",
                table: "RoutSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_TrucksTypes_TrucksTypeId",
                table: "RoutSteps");

            migrationBuilder.DropIndex(
                name: "IX_RoutSteps_GoodsDetailId",
                table: "RoutSteps");

            migrationBuilder.DropIndex(
                name: "IX_RoutSteps_TrailerTypeId",
                table: "RoutSteps");

            migrationBuilder.DropIndex(
                name: "IX_RoutSteps_TrucksTypeId",
                table: "RoutSteps");

            migrationBuilder.DropColumn(
                name: "GoodsDetailId",
                table: "RoutSteps");

            migrationBuilder.DropColumn(
                name: "TrailerTypeId",
                table: "RoutSteps");

            migrationBuilder.DropColumn(
                name: "TrucksTypeId",
                table: "RoutSteps");

            migrationBuilder.AddColumn<long>(
                name: "GoodsDetailId",
                table: "ShippingRequests",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrailerTypeId",
                table: "ShippingRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TrucksTypeId",
                table: "ShippingRequests",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_GoodsDetailId",
                table: "ShippingRequests",
                column: "GoodsDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_TrailerTypeId",
                table: "ShippingRequests",
                column: "TrailerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_TrucksTypeId",
                table: "ShippingRequests",
                column: "TrucksTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_GoodsDetails_GoodsDetailId",
                table: "ShippingRequests",
                column: "GoodsDetailId",
                principalTable: "GoodsDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
    }
}
