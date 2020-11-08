using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class maketruckstatusnullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trucks_TruckStatuses_TruckStatusId",
                table: "Trucks");

            migrationBuilder.AlterColumn<long>(
                name: "TruckStatusId",
                table: "Trucks",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_Trucks_TruckStatuses_TruckStatusId",
                table: "Trucks",
                column: "TruckStatusId",
                principalTable: "TruckStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trucks_TruckStatuses_TruckStatusId",
                table: "Trucks");

            migrationBuilder.AlterColumn<long>(
                name: "TruckStatusId",
                table: "Trucks",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Trucks_TruckStatuses_TruckStatusId",
                table: "Trucks",
                column: "TruckStatusId",
                principalTable: "TruckStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
