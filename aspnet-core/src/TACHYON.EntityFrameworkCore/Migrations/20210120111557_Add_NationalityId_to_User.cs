using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class Add_NationalityId_to_User : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nationality",
                table: "AbpUsers");

            migrationBuilder.AddColumn<int>(
                name: "NationalityId",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_NationalityId",
                table: "AbpUsers",
                column: "NationalityId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpUsers_Nationalities_NationalityId",
                table: "AbpUsers",
                column: "NationalityId",
                principalTable: "Nationalities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpUsers_Nationalities_NationalityId",
                table: "AbpUsers");

            migrationBuilder.DropIndex(
                name: "IX_AbpUsers_NationalityId",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "NationalityId",
                table: "AbpUsers");

            migrationBuilder.AddColumn<string>(
                name: "Nationality",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
