using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Fix_ShippingRequestId_To_RoutStep_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_Routes_ShippingRequestId",
                table: "RoutSteps");

            migrationBuilder.AlterColumn<long>(
                name: "ShippingRequestId",
                table: "RoutSteps",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_ShippingRequests_ShippingRequestId",
                table: "RoutSteps",
                column: "ShippingRequestId",
                principalTable: "ShippingRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_ShippingRequests_ShippingRequestId",
                table: "RoutSteps");

            migrationBuilder.AlterColumn<int>(
                name: "ShippingRequestId",
                table: "RoutSteps",
                type: "int",
                nullable: true,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_Routes_ShippingRequestId",
                table: "RoutSteps",
                column: "ShippingRequestId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
