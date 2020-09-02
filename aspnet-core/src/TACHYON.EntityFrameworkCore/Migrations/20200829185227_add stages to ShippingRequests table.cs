using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addstagestoShippingRequeststable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_Routes_RouteId",
                table: "ShippingRequests");

            migrationBuilder.AlterColumn<int>(
                name: "RouteId",
                table: "ShippingRequests",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfDrops",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "StageOneFinish",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "StageThreeFinish",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "StageTowFinish",
                table: "ShippingRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_Routes_RouteId",
                table: "ShippingRequests",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRequests_Routes_RouteId",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "NumberOfDrops",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "StageOneFinish",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "StageThreeFinish",
                table: "ShippingRequests");

            migrationBuilder.DropColumn(
                name: "StageTowFinish",
                table: "ShippingRequests");

            migrationBuilder.AlterColumn<int>(
                name: "RouteId",
                table: "ShippingRequests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRequests_Routes_RouteId",
                table: "ShippingRequests",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
