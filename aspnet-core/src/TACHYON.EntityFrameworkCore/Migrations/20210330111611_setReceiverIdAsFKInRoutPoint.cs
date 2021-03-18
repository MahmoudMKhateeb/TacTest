using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class setReceiverIdAsFKInRoutPoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RoutPoints_ReceiverId",
                table: "RoutPoints",
                column: "ReceiverId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoutPoints_Receivers_ReceiverId",
                table: "RoutPoints",
                column: "ReceiverId",
                principalTable: "Receivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoutPoints_Receivers_ReceiverId",
                table: "RoutPoints");

            migrationBuilder.DropIndex(
                name: "IX_RoutPoints_ReceiverId",
                table: "RoutPoints");
        }
    }
}
