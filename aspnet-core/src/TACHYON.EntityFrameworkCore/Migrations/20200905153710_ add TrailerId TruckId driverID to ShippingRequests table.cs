using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addTrailerIdTruckIddriverIDtoShippingRequeststable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AssignedDriverUserId",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "AssignedTrailerId",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedTruckId",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_AssignedDriverUserId",
                table: "ShippingRequests",
                column: "AssignedDriverUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_AssignedTrailerId",
                table: "ShippingRequests",
                column: "AssignedTrailerId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRequests_AssignedTruckId",
                table: "ShippingRequests",
                column: "AssignedTruckId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_AbpUsers_AssignedDriverUserId",
                table: "ShippingRequests",
                column: "AssignedDriverUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_Trailers_AssignedTrailerId",
                table: "ShippingRequests",
                column: "AssignedTrailerId",
                principalTable: "Trailers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_Trucks_AssignedTruckId",
                table: "ShippingRequests",
                column: "AssignedTruckId",
                principalTable: "Trucks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_AbpUsers_AssignedDriverUserId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_Trailers_AssignedTrailerId",
                table: "ShippingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_Trucks_AssignedTruckId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_AssignedDriverUserId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_AssignedTrailerId",
                table: "ShippingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShippingRequests_AssignedTruckId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "AssignedDriverUserId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "AssignedTrailerId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "AssignedTruckId",
                table: "ShippingRequests");
        }
    }
}
