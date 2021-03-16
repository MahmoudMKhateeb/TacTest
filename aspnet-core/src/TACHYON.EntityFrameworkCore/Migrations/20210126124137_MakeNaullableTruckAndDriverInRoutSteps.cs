using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class MakeNaullableTruckAndDriverInRoutSteps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_AbpUsers_AssignedDriverUserId",
                table: "RoutSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_Trucks_AssignedTruckId",
                table: "RoutSteps");

            migrationBuilder.AlterColumn<long>(
                name: "AssignedTruckId",
                table: "RoutSteps",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "AssignedDriverUserId",
                table: "RoutSteps",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_AbpUsers_AssignedDriverUserId",
                table: "RoutSteps",
                column: "AssignedDriverUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_Trucks_AssignedTruckId",
                table: "RoutSteps",
                column: "AssignedTruckId",
                principalTable: "Trucks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_AbpUsers_AssignedDriverUserId",
                table: "RoutSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_RoutSteps_Trucks_AssignedTruckId",
                table: "RoutSteps");

            migrationBuilder.AlterColumn<long>(
                name: "AssignedTruckId",
                table: "RoutSteps",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "AssignedDriverUserId",
                table: "RoutSteps",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutSteps_AbpUsers_AssignedDriverUserId",
                table: "RoutSteps",
                column: "AssignedDriverUserId",
                principalTable: "AbpUsers",
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
    }
}
