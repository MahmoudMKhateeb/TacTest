using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Add_ShippingRequestId_To_RoutStep_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_Routes_RouteId",
                table: "RoutSteps");

            migrationBuilder.AlterColumn<int>(
                name: "RouteId",
                table: "RoutSteps",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ShippingRequestId",
                table: "RoutSteps",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoutSteps_ShippingRequestId",
                table: "RoutSteps",
                column: "ShippingRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_Routes_RouteId",
                table: "RoutSteps",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_Routes_ShippingRequestId",
                table: "RoutSteps",
                column: "ShippingRequestId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_Routes_RouteId",
                table: "RoutSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_Routes_ShippingRequestId",
                table: "RoutSteps");

            migrationBuilder.DropIndex(
                name: "IX_RoutSteps_ShippingRequestId",
                table: "RoutSteps");

            migrationBuilder.DropColumn(
                name: "ShippingRequestId",
                table: "RoutSteps");

            migrationBuilder.AlterColumn<int>(
                name: "RouteId",
                table: "RoutSteps",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_Routes_RouteId",
                table: "RoutSteps",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
