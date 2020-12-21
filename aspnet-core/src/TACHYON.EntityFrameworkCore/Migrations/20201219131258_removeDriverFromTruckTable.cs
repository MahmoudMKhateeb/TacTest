using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class removeDriverFromTruckTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trucks_AbpUsers_Driver1UserId",
                table: "Trucks");

            migrationBuilder.DropIndex(
                name: "IX_Trucks_Driver1UserId",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "Driver1UserId",
                table: "Trucks");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Driver1UserId",
                table: "Trucks",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_Driver1UserId",
                table: "Trucks",
                column: "Driver1UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trucks_AbpUsers_Driver1UserId",
                table: "Trucks",
                column: "Driver1UserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
