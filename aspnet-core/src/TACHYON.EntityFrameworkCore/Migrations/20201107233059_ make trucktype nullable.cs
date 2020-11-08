using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class maketrucktypenullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trucks_TrucksTypes_TrucksTypeId",
                table: "Trucks");

            migrationBuilder.AlterColumn<long>(
                name: "TrucksTypeId",
                table: "Trucks",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_Trucks_TrucksTypes_TrucksTypeId",
                table: "Trucks",
                column: "TrucksTypeId",
                principalTable: "TrucksTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trucks_TrucksTypes_TrucksTypeId",
                table: "Trucks");

            migrationBuilder.AlterColumn<long>(
                name: "TrucksTypeId",
                table: "Trucks",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Trucks_TrucksTypes_TrucksTypeId",
                table: "Trucks",
                column: "TrucksTypeId",
                principalTable: "TrucksTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
