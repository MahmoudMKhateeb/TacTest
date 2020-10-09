using Microsoft.EntityFrameworkCore.Migrations;

namespace TACHYON.Migrations
{
    public partial class addEditionIdtoDocumentFileTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EditionId",
                table: "DocumentFiles",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFiles_EditionId",
                table: "DocumentFiles",
                column: "EditionId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentFiles_AbpEditions_EditionId",
                table: "DocumentFiles",
                column: "EditionId",
                principalTable: "AbpEditions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentFiles_AbpEditions_EditionId",
                table: "DocumentFiles");

            migrationBuilder.DropIndex(
                name: "IX_DocumentFiles_EditionId",
                table: "DocumentFiles");

            migrationBuilder.DropColumn(
                name: "EditionId",
                table: "DocumentFiles");
        }
    }
}
