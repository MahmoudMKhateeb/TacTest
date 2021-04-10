using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addReceiverNotetodropoffpoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ReceiverFkId",
                table: "RoutPoints",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiverNote",
                table: "RoutPoints",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoutPoints_ReceiverFkId",
                table: "RoutPoints",
                column: "ReceiverFkId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoutPoints_AbpUsers_ReceiverFkId",
                table: "RoutPoints",
                column: "ReceiverFkId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoutPoints_AbpUsers_ReceiverFkId",
                table: "RoutPoints");

            migrationBuilder.DropIndex(
                name: "IX_RoutPoints_ReceiverFkId",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "ReceiverFkId",
                table: "RoutPoints");

            migrationBuilder.DropColumn(
                name: "ReceiverNote",
                table: "RoutPoints");
        }
    }
}
