using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class RoutePointTranstionUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoutePointTranstions_RoutPoints_FromPointId",
                table: "RoutePointTranstions");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutePointTranstions_RoutPoints_ToPointId",
                table: "RoutePointTranstions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoutePointTranstions",
                table: "RoutePointTranstions");

            migrationBuilder.RenameTable(
                name: "RoutePointTranstions",
                newName: "RoutePointTransitions");

            migrationBuilder.RenameIndex(
                name: "IX_RoutePointTranstions_ToPointId",
                table: "RoutePointTransitions",
                newName: "IX_RoutePointTransitions_ToPointId");

            migrationBuilder.RenameIndex(
                name: "IX_RoutePointTranstions_FromPointId",
                table: "RoutePointTransitions",
                newName: "IX_RoutePointTransitions_FromPointId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoutePointTransitions",
                table: "RoutePointTransitions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoutePointTransitions_RoutPoints_FromPointId",
                table: "RoutePointTransitions",
                column: "FromPointId",
                principalTable: "RoutPoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutePointTransitions_RoutPoints_ToPointId",
                table: "RoutePointTransitions",
                column: "ToPointId",
                principalTable: "RoutPoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoutePointTransitions_RoutPoints_FromPointId",
                table: "RoutePointTransitions");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutePointTransitions_RoutPoints_ToPointId",
                table: "RoutePointTransitions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoutePointTransitions",
                table: "RoutePointTransitions");

            migrationBuilder.RenameTable(
                name: "RoutePointTransitions",
                newName: "RoutePointTranstions");

            migrationBuilder.RenameIndex(
                name: "IX_RoutePointTransitions_ToPointId",
                table: "RoutePointTranstions",
                newName: "IX_RoutePointTranstions_ToPointId");

            migrationBuilder.RenameIndex(
                name: "IX_RoutePointTransitions_FromPointId",
                table: "RoutePointTranstions",
                newName: "IX_RoutePointTranstions_FromPointId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoutePointTranstions",
                table: "RoutePointTranstions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoutePointTranstions_RoutPoints_FromPointId",
                table: "RoutePointTranstions",
                column: "FromPointId",
                principalTable: "RoutPoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutePointTranstions_RoutPoints_ToPointId",
                table: "RoutePointTranstions",
                column: "ToPointId",
                principalTable: "RoutPoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
