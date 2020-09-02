using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addUnitOfMeasureIdtoGoodsDetailstable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AssignedDriverUserId",
                table: "RoutSteps",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "AssignedTrailerId",
                table: "RoutSteps",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedTruckId",
                table: "RoutSteps",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "UnitOfMeasureId",
                table: "GoodsDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RoutSteps_AssignedDriverUserId",
                table: "RoutSteps",
                column: "AssignedDriverUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutSteps_AssignedTrailerId",
                table: "RoutSteps",
                column: "AssignedTrailerId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutSteps_AssignedTruckId",
                table: "RoutSteps",
                column: "AssignedTruckId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsDetails_UnitOfMeasureId",
                table: "GoodsDetails",
                column: "UnitOfMeasureId");

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsDetails_UnitOfMeasures_UnitOfMeasureId",
                table: "GoodsDetails",
                column: "UnitOfMeasureId",
                principalTable: "UnitOfMeasures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_AbpUsers_AssignedDriverUserId",
                table: "RoutSteps",
                column: "AssignedDriverUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_Trailers_AssignedTrailerId",
                table: "RoutSteps",
                column: "AssignedTrailerId",
                principalTable: "Trailers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_Trucks_AssignedTruckId",
                table: "RoutSteps",
                column: "AssignedTruckId",
                principalTable: "Trucks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GoodsDetails_UnitOfMeasures_UnitOfMeasureId",
                table: "GoodsDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_AbpUsers_AssignedDriverUserId",
                table: "RoutSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_Trailers_AssignedTrailerId",
                table: "RoutSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_Trucks_AssignedTruckId",
                table: "RoutSteps");

            migrationBuilder.DropIndex(
                name: "IX_RoutSteps_AssignedDriverUserId",
                table: "RoutSteps");

            migrationBuilder.DropIndex(
                name: "IX_RoutSteps_AssignedTrailerId",
                table: "RoutSteps");

            migrationBuilder.DropIndex(
                name: "IX_RoutSteps_AssignedTruckId",
                table: "RoutSteps");

            migrationBuilder.DropIndex(
                name: "IX_GoodsDetails_UnitOfMeasureId",
                table: "GoodsDetails");

            migrationBuilder.DropColumn(
                name: "AssignedDriverUserId",
                table: "RoutSteps");

            migrationBuilder.DropColumn(
                name: "AssignedTrailerId",
                table: "RoutSteps");

            migrationBuilder.DropColumn(
                name: "AssignedTruckId",
                table: "RoutSteps");

            migrationBuilder.DropColumn(
                name: "UnitOfMeasureId",
                table: "GoodsDetails");
        }
    }
}
